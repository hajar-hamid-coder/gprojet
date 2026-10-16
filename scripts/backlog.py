#!/usr/bin/env python3
"""Moteur du backlog : lecture, transitions de statut, hypothèses, contrôle d'arrêt.

Source unique des statuts : backlog/BACKLOG.md et backlog/HYPOTHESES.md.
Claude ne modifie jamais ces fichiers à la main : il passe par ce script.

Commandes Claude      : etat, contexte, journal, suivante, voir, tache, note, hypothese-ajouter, stop-check
Commandes utilisateur : lancer-phase, valider-phase, refuser-tache, repondre-hypothese
"""
from __future__ import annotations

import argparse
import hashlib
import json
import os
import re
import subprocess
import sys
from dataclasses import dataclass, field
from datetime import datetime
from pathlib import Path

RACINE = Path(__file__).resolve().parent.parent
BACKLOG = RACINE / "backlog" / "BACKLOG.md"
HYPOTHESES = RACINE / "backlog" / "HYPOTHESES.md"
JOURNAL = RACINE / "backlog" / "JOURNAL.md"
ETAT_STOP = RACINE / ".claude" / "etat-stop.json"

STATUTS_TACHE = ("NON_FAIT", "EN_COURS", "A_VALIDER", "VALIDE", "BLOQUE")
STATUTS_PHASE = ("NON_DEMARREE", "EN_COURS", "A_VALIDER", "VALIDEE")
TERMINES = ("A_VALIDER", "VALIDE")
COMMANDES_UTILISATEUR = ("lancer-phase", "valider-phase", "refuser-tache", "repondre-hypothese")
MAX_ARRETS_SANS_PROGRES = 3

RE_PHASE = re.compile(r"^## Phase (P\d+) · (.+)$")
RE_META_PHASE = re.compile(r"^> \*\*Statut phase\*\* : (\w+) · \*\*Modèle conseillé\*\* : (\w+)\s*$")
RE_TACHE = re.compile(r"^### (P\d+-T\d+) · (.+)$")
RE_META_TACHE = re.compile(
    r"^> \*\*Statut\*\* : (\w+) · \*\*Modèle\*\* : (\w+) · \*\*Ordre\*\* : (\d+) · \*\*Dépend de\*\* : (.+?)\s*$")
RE_HYP = re.compile(r"^### (H-\d+) · (P\d+-T\d+)\s*$")
RE_META_HYP = re.compile(r"^> \*\*Statut\*\* : (\w+) · \*\*Bloquante\*\* : (oui|non) · \*\*Date\*\* : (.+?)\s*$")
RESUME_DEBUT, RESUME_FIN = "<!-- RESUME:DEBUT -->", "<!-- RESUME:FIN -->"


class ErreurBacklog(Exception):
    pass


@dataclass
class Tache:
    id: str
    titre: str
    statut: str
    modele: str
    ordre: int
    depend: list[str]
    ligne_meta: int
    debut: int
    fin: int = 0


@dataclass
class Phase:
    id: str
    titre: str
    statut: str
    modele: str
    ligne_meta: int
    taches: list[Tache] = field(default_factory=list)


@dataclass
class Hypothese:
    id: str
    tache: str
    statut: str
    bloquante: bool
    ligne_meta: int
    debut: int
    fin: int = 0


def lire(chemin: Path) -> list[str]:
    return chemin.read_text(encoding="utf-8").split("\n")


def ecrire(chemin: Path, lignes: list[str]) -> None:
    chemin.write_text("\n".join(lignes), encoding="utf-8")


def maintenant() -> str:
    return datetime.now().strftime("%Y-%m-%d %H:%M")


# ----------------------------------------------------------------- lecture

def charger_backlog(lignes: list[str]) -> list[Phase]:
    phases: list[Phase] = []
    tache_courante: Tache | None = None
    for i, ligne in enumerate(lignes):
        if m := RE_PHASE.match(ligne):
            if tache_courante:
                tache_courante.fin = i
                tache_courante = None
            meta = RE_META_PHASE.match(lignes[i + 1]) if i + 1 < len(lignes) else None
            if not meta:
                raise ErreurBacklog(f"Ligne {i + 2} : métadonnées de phase {m[1]} invalides")
            if meta[1] not in STATUTS_PHASE:
                raise ErreurBacklog(f"Phase {m[1]} : statut inconnu {meta[1]}")
            phases.append(Phase(m[1], m[2].strip(), meta[1], meta[2], i + 1))
        elif m := RE_TACHE.match(ligne):
            if tache_courante:
                tache_courante.fin = i
            if not phases:
                raise ErreurBacklog(f"Tâche {m[1]} hors phase")
            meta = RE_META_TACHE.match(lignes[i + 1]) if i + 1 < len(lignes) else None
            if not meta:
                raise ErreurBacklog(f"Ligne {i + 2} : métadonnées de tâche {m[1]} invalides")
            if meta[1] not in STATUTS_TACHE:
                raise ErreurBacklog(f"Tâche {m[1]} : statut inconnu {meta[1]}")
            dep = [] if meta[4].strip() in ("—", "-", "") else [d.strip() for d in meta[4].split(",")]
            tache_courante = Tache(m[1], m[2].strip(), meta[1], meta[2], int(meta[3]), dep, i + 1, i)
            phases[-1].taches.append(tache_courante)
        elif ligne.startswith("## ") and tache_courante:
            tache_courante.fin = i
            tache_courante = None
    if tache_courante:
        tache_courante.fin = len(lignes)
    return phases


def charger_hypotheses(lignes: list[str]) -> list[Hypothese]:
    hyps: list[Hypothese] = []
    for i, ligne in enumerate(lignes):
        if m := RE_HYP.match(ligne):
            if hyps and not hyps[-1].fin:
                hyps[-1].fin = i
            meta = RE_META_HYP.match(lignes[i + 1]) if i + 1 < len(lignes) else None
            if not meta:
                raise ErreurBacklog(f"HYPOTHESES.md ligne {i + 2} : métadonnées invalides")
            hyps.append(Hypothese(m[1], m[2], meta[1], meta[2] == "oui", i + 1, i))
    if hyps and not hyps[-1].fin:
        hyps[-1].fin = len(lignes)
    return hyps


def index_taches(phases: list[Phase]) -> dict[str, Tache]:
    return {t.id: t for p in phases for t in p.taches}


def phase_de(phases: list[Phase], tache_id: str) -> Phase:
    for p in phases:
        if any(t.id == tache_id for t in p.taches):
            return p
    raise ErreurBacklog(f"Tâche inconnue : {tache_id}")


def phase_par_id(phases: list[Phase], phase_id: str) -> Phase:
    for p in phases:
        if p.id == phase_id:
            return p
    raise ErreurBacklog(f"Phase inconnue : {phase_id}")


def phase_active(phases: list[Phase]) -> Phase | None:
    return next((p for p in phases if p.statut in ("EN_COURS", "A_VALIDER")), None)


def dependances_ok(t: Tache, idx: dict[str, Tache]) -> list[str]:
    """Renvoie les dépendances non terminées."""
    manquantes = []
    for d in t.depend:
        if d not in idx:
            raise ErreurBacklog(f"{t.id} dépend de {d}, inconnue")
        if idx[d].statut not in TERMINES:
            manquantes.append(f"{d} ({idx[d].statut})")
    return manquantes


def executables(p: Phase, idx: dict[str, Tache]) -> list[Tache]:
    return sorted((t for t in p.taches if t.statut == "NON_FAIT" and not dependances_ok(t, idx)),
                  key=lambda t: t.ordre)


# ----------------------------------------------------------------- écriture

def meta_tache(t: Tache) -> str:
    dep = ", ".join(t.depend) if t.depend else "—"
    return f"> **Statut** : {t.statut} · **Modèle** : {t.modele} · **Ordre** : {t.ordre} · **Dépend de** : {dep}"


def meta_phase(p: Phase) -> str:
    return f"> **Statut phase** : {p.statut} · **Modèle conseillé** : {p.modele}"


ICONES = {"NON_FAIT": "⬜", "EN_COURS": "🔄", "A_VALIDER": "🟡", "VALIDE": "✅", "BLOQUE": "⛔",
          "NON_DEMARREE": "⬜", "VALIDEE": "✅"}


def resume(phases: list[Phase]) -> list[str]:
    lignes = [RESUME_DEBUT, "", f"_Mis à jour le {maintenant()} par scripts/backlog.py — ne pas modifier à la main._", "",
              "| Phase | Titre | Statut | Modèle | Avancement |", "|---|---|---|---|---|"]
    for p in phases:
        faits = sum(t.statut in TERMINES for t in p.taches)
        lignes.append(f"| {p.id} | {p.titre} | {ICONES.get(p.statut, '')} {p.statut} | {p.modele} | {faits}/{len(p.taches)} |")
    lignes += ["", RESUME_FIN]
    return lignes


def sauver_backlog(lignes: list[str], phases: list[Phase]) -> None:
    for p in phases:
        lignes[p.ligne_meta] = meta_phase(p)
        for t in p.taches:
            lignes[t.ligne_meta] = meta_tache(t)
    if RESUME_DEBUT in lignes and RESUME_FIN in lignes:
        a, b = lignes.index(RESUME_DEBUT), lignes.index(RESUME_FIN)
        lignes[a:b + 1] = resume(phases)
    ecrire(BACKLOG, lignes)
    regenerer_excel()


def regenerer_excel() -> None:
    """Vue Excel (backlog/BACKLOG.xlsx) toujours alignée sur le Markdown ; un échec n'annule jamais la transition."""
    try:
        import backlog_excel
        backlog_excel.exporter()
    except PermissionError:
        print("AVERTISSEMENT : BACKLOG.xlsx est ouvert dans Excel, non régénéré "
              "(python scripts/backlog_excel.py après fermeture).", file=sys.stderr)
    except ImportError as e:
        print(f"AVERTISSEMENT : export Excel impossible ({e}) : pip install openpyxl.", file=sys.stderr)


def journaliser(texte: str) -> None:
    with JOURNAL.open("a", encoding="utf-8") as f:
        f.write(f"\n- **{maintenant()}** — {texte}")


def recalculer_phase(p: Phase) -> None:
    if p.statut == "EN_COURS" and p.taches and all(t.statut in TERMINES for t in p.taches):
        p.statut = "A_VALIDER"
        journaliser(f"Phase {p.id} terminée par Claude : en attente de validation utilisateur (`/valider-phase {p.id}`).")


# ----------------------------------------------------------------- commandes

def cmd_etat(_args) -> str:
    phases = charger_backlog(lire(BACKLOG))
    return "\n".join(resume(phases)[3:-2])


def cmd_contexte(_args) -> str:
    phases = charger_backlog(lire(BACKLOG))
    idx = index_taches(phases)
    hyps = charger_hypotheses(lire(HYPOTHESES))
    ouvertes = [h for h in hyps if h.statut == "OUVERTE"]
    p = phase_active(phases)
    out = ["# État du backlog (injecté au démarrage de session)", ""]
    if not p:
        derniere = next((x for x in reversed(phases) if x.statut == "VALIDEE"), None)
        prochaine = next((x for x in phases if x.statut == "NON_DEMARREE"), None)
        out.append(f"Aucune phase en cours. Dernière phase validée : {derniere.id if derniere else 'aucune'}.")
        if prochaine:
            out.append(f"Prochaine phase : **{prochaine.id} · {prochaine.titre}** (modèle conseillé : {prochaine.modele}).")
        out.append("**Ne commence aucune tâche** : attends que l'utilisateur lance `/lancer-phase`.")
    else:
        out.append(f"Phase active : **{p.id} · {p.titre}** — statut **{p.statut}** — modèle conseillé : {p.modele}")
        out.append("")
        for t in sorted(p.taches, key=lambda x: x.ordre):
            out.append(f"- {ICONES[t.statut]} {t.id} [{t.modele}] {t.titre} — {t.statut}")
        if p.statut == "A_VALIDER":
            out.append("")
            out.append(f"Toutes les tâches sont faites. Demande à l'utilisateur de valider avec `/valider-phase {p.id}` "
                       "ou de refuser une tâche avec `/refuser-tache`. Ne lance rien d'autre.")
        else:
            en_cours = [t for t in p.taches if t.statut == "EN_COURS"]
            suiv = executables(p, idx)
            out.append("")
            if en_cours:
                t = en_cours[0]
                out.append(f"⚠️ **Tâche interrompue : {t.id}** (modèle {t.modele}). Applique la **procédure de reprise** "
                           "(skill `reprendre`) avant toute autre action : constater l'état Git, vérifier, puis terminer la tâche "
                           "sans repartir de zéro.")
                notes = [l for l in lire(JOURNAL) if t.id in l][-6:]
                if notes:
                    out += ["", f"Dernières traces de {t.id} dans le journal :"] + notes
            elif suiv:
                out.append(f"Prochaine tâche : **{suiv[0].id}** (modèle {suiv[0].modele}). Suis la procédure du skill `executer-tache`.")
            else:
                out.append("Aucune tâche exécutable : tâches bloquées par des hypothèses. Informe l'utilisateur.")
    if ouvertes:
        out += ["", f"Hypothèses ouvertes : {len(ouvertes)} — " +
                ", ".join(f"{h.id} ({h.tache}{', bloquante' if h.bloquante else ''})" for h in ouvertes)]
    out += ["", "## Environnement", *etat_environnement()]
    ETAT_STOP.unlink(missing_ok=True)  # nouvelle session : le compteur anti-boucle repart de zéro
    return "\n".join(out)


def git(*args: str) -> str | None:
    try:
        r = subprocess.run(["git", *args], cwd=RACINE, capture_output=True, text=True, encoding="utf-8", timeout=15)
    except (OSError, subprocess.TimeoutExpired):
        return None
    return r.stdout.rstrip() if r.returncode == 0 else None  # rstrip : garder les colonnes de « status --porcelain »


def etat_environnement() -> list[str]:
    lignes = []
    branche = git("branch", "--show-current")
    if branche is None:
        lignes.append("- Git : pas encore de dépôt (créé par P0-T01).")
    else:
        modifs = [l[3:] for l in (git("status", "--porcelain") or "").splitlines()]
        pilotage = [m for m in modifs if m.startswith("backlog/") or m.startswith(".claude/etat-stop")]
        code = [m for m in modifs if m not in pilotage]
        dernier = git("log", "-1", "--format=%h %s") or "aucun commit"
        lignes.append(f"- Git : branche `{branche}` ; dernier commit : {dernier}")
        lignes.append("- Code non commité : " + (f"**{len(code)} fichier(s)** : {', '.join(code[:8])}{' …' if len(code) > 8 else ''}"
                                                if code else "aucun"))
        if pilotage:
            lignes.append(f"- Fichiers de pilotage modifiés ({len(pilotage)}) : normal, ils changent à chaque transition ; "
                          "ils partent avec le prochain commit de tâche — **ne jamais les stasher ni les restaurer**.")
    lignes.append("- Base de dev : variable `ConnectionStrings__Gp` "
                  + ("présente" if os.environ.get("ConnectionStrings__Gp") else "**absente** (vérifier `.claude/settings.local.json`)"))
    if (RACINE / ".pgtest" / "data").exists():
        lignes.append("- Instance de test : un dossier `.pgtest/` subsiste d'une session précédente → `python scripts/pg-test.py arreter`.")
    xlsx = RACINE / "backlog" / "BACKLOG.xlsx"
    if xlsx.exists() and xlsx.stat().st_mtime < BACKLOG.stat().st_mtime:
        lignes.append("- `BACKLOG.xlsx` est en retard sur le Markdown → `python scripts/backlog_excel.py`.")
    return lignes


def cmd_journal(args) -> str:
    """Dernières lignes du journal (utilisé par la procédure de reprise)."""
    entrees = [l for l in lire(JOURNAL) if l.startswith("- ")]
    return "\n".join(entrees[-args.n:])


def cmd_note(args) -> str:
    """Trace de progression d'une tâche (sert à la reprise après interruption)."""
    phases = charger_backlog(lire(BACKLOG))
    if args.id not in index_taches(phases):
        raise ErreurBacklog(f"Tâche inconnue : {args.id}")
    journaliser(f"NOTE {args.id} — {args.texte}")
    return f"Note enregistrée pour {args.id}."


def cmd_suivante(_args) -> str:
    phases = charger_backlog(lire(BACKLOG))
    p = phase_active(phases)
    if not p or p.statut != "EN_COURS":
        return "AUCUNE : pas de phase EN_COURS."
    idx = index_taches(phases)
    en_cours = [t for t in p.taches if t.statut == "EN_COURS"]
    if en_cours:
        t = en_cours[0]
        return f"{t.id}|{t.modele}|{t.titre}|REPRENDRE"
    suiv = executables(p, idx)
    if not suiv:
        return "AUCUNE : toutes les tâches restantes sont bloquées ou faites."
    t = suiv[0]
    return f"{t.id}|{t.modele}|{t.titre}|NOUVELLE"


def cmd_voir(args) -> str:
    lignes = lire(BACKLOG)
    idx = index_taches(charger_backlog(lignes))
    if args.id not in idx:
        raise ErreurBacklog(f"Tâche inconnue : {args.id}")
    t = idx[args.id]
    bloc = "\n".join(lignes[t.debut:t.fin]).rstrip()
    hyps = [h for h in charger_hypotheses(lire(HYPOTHESES)) if h.tache == t.id]
    if hyps:
        hl = lire(HYPOTHESES)
        bloc += "\n\n#### Hypothèses liées\n" + "\n\n".join("\n".join(hl[h.debut:h.fin]).rstrip() for h in hyps)
    return bloc


def cmd_tache(args) -> str:
    lignes = lire(BACKLOG)
    phases = charger_backlog(lignes)
    idx = index_taches(phases)
    if args.id not in idx:
        raise ErreurBacklog(f"Tâche inconnue : {args.id}")
    t, p, cible = idx[args.id], phase_de(phases, args.id), args.statut
    if cible not in ("EN_COURS", "A_VALIDER", "BLOQUE", "NON_FAIT"):
        raise ErreurBacklog(f"Statut {cible} réservé à l'utilisateur ou inconnu.")
    if p.statut != "EN_COURS":
        raise ErreurBacklog(f"La phase {p.id} est {p.statut} : aucune transition possible.")
    ouvertes_bloquantes = [h for h in charger_hypotheses(lire(HYPOTHESES))
                           if h.tache == t.id and h.statut == "OUVERTE" and h.bloquante]
    autorisees = {
        ("NON_FAIT", "EN_COURS"), ("EN_COURS", "A_VALIDER"), ("EN_COURS", "BLOQUE"),
        ("EN_COURS", "NON_FAIT"), ("BLOQUE", "EN_COURS"), ("A_VALIDER", "EN_COURS"),
    }
    if (t.statut, cible) not in autorisees:
        raise ErreurBacklog(f"Transition {t.statut} → {cible} interdite pour {t.id}.")
    if cible == "EN_COURS":
        autre = [x for x in p.taches if x.statut == "EN_COURS" and x.id != t.id]
        if autre:
            raise ErreurBacklog(f"{autre[0].id} est déjà EN_COURS : termine-la avant d'en démarrer une autre.")
        if manquantes := dependances_ok(t, idx):
            raise ErreurBacklog(f"Ordre d'exécution : {t.id} attend {', '.join(manquantes)}.")
        if ouvertes_bloquantes:
            raise ErreurBacklog(f"{t.id} a des hypothèses bloquantes ouvertes : "
                                f"{', '.join(h.id for h in ouvertes_bloquantes)}.")
    if cible == "BLOQUE" and not ouvertes_bloquantes:
        raise ErreurBacklog("Pour bloquer une tâche, crée d'abord une hypothèse bloquante (hypothese-ajouter --bloquante oui).")
    if cible == "A_VALIDER" and not args.preuve:
        raise ErreurBacklog("--preuve obligatoire : résume le résultat du test de validation exécuté.")
    ancien, t.statut = t.statut, cible
    recalculer_phase(p)
    sauver_backlog(lignes, phases)
    texte = f"{t.id} {ancien} → {cible}"
    if args.preuve:
        texte += f" — preuve : {args.preuve}"
    journaliser(texte)
    return texte + (f"\nPhase {p.id} → {p.statut}" if p.statut == "A_VALIDER" else "")


def cmd_hypothese_ajouter(args) -> str:
    phases = charger_backlog(lire(BACKLOG))
    if args.tache not in index_taches(phases):
        raise ErreurBacklog(f"Tâche inconnue : {args.tache}")
    lignes = lire(HYPOTHESES)
    hyps = charger_hypotheses(lignes)
    num = max((int(h.id[2:]) for h in hyps), default=0) + 1
    hid = f"H-{num:03d}"
    bloc = ["", f"### {hid} · {args.tache}",
            f"> **Statut** : OUVERTE · **Bloquante** : {args.bloquante} · **Date** : {maintenant()}",
            f"**Question** : {args.question}",
            f"**Hypothèse retenue par défaut** : {args.hypothese}",
            f"**Impact si l'hypothèse est fausse** : {args.impact}",
            "**Réponse utilisateur** : _en attente_"]
    ecrire(HYPOTHESES, lignes + bloc)
    regenerer_excel()
    journaliser(f"Hypothèse {hid} créée pour {args.tache} (bloquante : {args.bloquante}).")
    return hid


def cmd_lancer_phase(args) -> str:
    lignes = lire(BACKLOG)
    phases = charger_backlog(lignes)
    p = phase_par_id(phases, args.phase)
    if p.statut != "NON_DEMARREE":
        raise ErreurBacklog(f"{p.id} est déjà {p.statut}.")
    if (a := phase_active(phases)) is not None:
        raise ErreurBacklog(f"{a.id} est {a.statut} : valide-la d'abord.")
    for prec in phases[:phases.index(p)]:
        if prec.statut != "VALIDEE":
            raise ErreurBacklog(f"Ordre des phases : {prec.id} n'est pas VALIDEE.")
    p.statut = "EN_COURS"
    sauver_backlog(lignes, phases)
    ETAT_STOP.unlink(missing_ok=True)
    journaliser(f"Phase {p.id} lancée par l'utilisateur.")
    return f"Phase {p.id} · {p.titre} lancée (modèle conseillé : {p.modele}, {len(p.taches)} tâches)."


def cmd_valider_phase(args) -> str:
    lignes = lire(BACKLOG)
    phases = charger_backlog(lignes)
    p = phase_par_id(phases, args.phase)
    if p.statut != "A_VALIDER":
        raise ErreurBacklog(f"{p.id} est {p.statut} : seule une phase A_VALIDER peut être validée.")
    for t in p.taches:
        t.statut = "VALIDE"
    p.statut = "VALIDEE"
    sauver_backlog(lignes, phases)
    journaliser(f"Phase {p.id} VALIDÉE par l'utilisateur.")
    suivante = next((x for x in phases if x.statut == "NON_DEMARREE"), None)
    return f"Phase {p.id} validée." + (f" Prochaine : {suivante.id} · {suivante.titre} "
                                      f"(modèle conseillé : {suivante.modele})." if suivante else " Projet terminé.")


def cmd_refuser_tache(args) -> str:
    lignes = lire(BACKLOG)
    phases = charger_backlog(lignes)
    idx = index_taches(phases)
    if args.id not in idx:
        raise ErreurBacklog(f"Tâche inconnue : {args.id}")
    t, p = idx[args.id], phase_de(phases, args.id)
    if t.statut != "A_VALIDER" or p.statut not in ("EN_COURS", "A_VALIDER"):
        raise ErreurBacklog(f"{t.id} est {t.statut} (phase {p.statut}) : seule une tâche A_VALIDER d'une phase active peut être refusée.")
    lignes.insert(t.ligne_meta + 1, f"> **Retour utilisateur ({maintenant()})** : {args.motif}")
    phases = charger_backlog(lignes)  # les numéros de ligne ont bougé
    t, p = index_taches(phases)[args.id], phase_de(phases, args.id)
    t.statut, p.statut = "NON_FAIT", "EN_COURS"
    sauver_backlog(lignes, phases)
    ETAT_STOP.unlink(missing_ok=True)
    journaliser(f"{t.id} refusée par l'utilisateur : {args.motif}")
    return f"{t.id} renvoyée en NON_FAIT, phase {p.id} repassée EN_COURS."


def cmd_repondre_hypothese(args) -> str:
    hl = lire(HYPOTHESES)
    hyps = charger_hypotheses(hl)
    h = next((x for x in hyps if x.id == args.id), None)
    if not h:
        raise ErreurBacklog(f"Hypothèse inconnue : {args.id}")
    if h.statut != "OUVERTE":
        raise ErreurBacklog(f"{h.id} est déjà {h.statut}.")
    m = RE_META_HYP.match(hl[h.ligne_meta])
    hl[h.ligne_meta] = f"> **Statut** : REPONDUE · **Bloquante** : {m[2]} · **Date** : {m[3]}"
    for i in range(h.debut, h.fin):
        if hl[i].startswith("**Réponse utilisateur** :"):
            hl[i] = f"**Réponse utilisateur** ({maintenant()}) : {args.reponse}"
    ecrire(HYPOTHESES, hl)
    regenerer_excel()
    journaliser(f"Hypothèse {h.id} répondue : {args.reponse}")
    msg = f"{h.id} répondue."
    lignes = lire(BACKLOG)
    phases = charger_backlog(lignes)
    t = index_taches(phases).get(h.tache)
    restantes = [x for x in charger_hypotheses(lire(HYPOTHESES))
                 if x.tache == h.tache and x.statut == "OUVERTE" and x.bloquante]
    if t and t.statut == "BLOQUE" and not restantes:
        t.statut = "NON_FAIT"
        sauver_backlog(lignes, phases)
        ETAT_STOP.unlink(missing_ok=True)
        msg += f" {t.id} débloquée (NON_FAIT)."
    return msg


def cmd_stop_check(_args) -> str:
    """Hook Stop : empêche Claude de s'arrêter tant que la phase a du travail exécutable."""
    try:
        json.loads(sys.stdin.read() or "{}")
    except json.JSONDecodeError:
        pass
    phases = charger_backlog(lire(BACKLOG))
    p = phase_active(phases)
    if not p or p.statut != "EN_COURS":
        ETAT_STOP.unlink(missing_ok=True)
        return ""
    idx = index_taches(phases)
    restantes = [t for t in p.taches if t.statut == "EN_COURS"] or executables(p, idx)
    if not restantes:
        ETAT_STOP.unlink(missing_ok=True)
        return ""
    signature = hashlib.sha1("|".join(f"{t.id}:{t.statut}" for t in p.taches).encode()).hexdigest()
    etat = json.loads(ETAT_STOP.read_text(encoding="utf-8")) if ETAT_STOP.exists() else {}
    compte = etat.get("compte", 0) + 1 if etat.get("signature") == signature else 1
    ETAT_STOP.write_text(json.dumps({"signature": signature, "compte": compte}), encoding="utf-8")
    if compte > MAX_ARRETS_SANS_PROGRES:
        return ""  # garde-fou : aucun progrès malgré les relances, on laisse l'utilisateur reprendre la main
    t = restantes[0]
    raison = (f"Phase {p.id} non terminée. Prochaine tâche : {t.id} « {t.titre} » (modèle {t.modele}, statut {t.statut}). "
              "Continue de façon autonome avec le skill `executer-tache`. Si une précision manque, crée une "
              "hypothèse ; si elle est bloquante, passe la tâche en BLOQUE et enchaîne sur la suivante.")
    return json.dumps({"decision": "block", "reason": raison}, ensure_ascii=False)


def main() -> int:
    for flux in (sys.stdout, sys.stderr):
        flux.reconfigure(encoding="utf-8")
    ap = argparse.ArgumentParser(prog="backlog.py", description=__doc__)
    sub = ap.add_subparsers(dest="cmd", required=True)
    sub.add_parser("etat")
    sub.add_parser("contexte")
    sub.add_parser("suivante")
    sub.add_parser("stop-check")
    s = sub.add_parser("voir"); s.add_argument("id")
    s = sub.add_parser("note"); s.add_argument("id"); s.add_argument("texte")
    s = sub.add_parser("journal"); s.add_argument("n", nargs="?", type=int, default=15)
    s = sub.add_parser("tache"); s.add_argument("id"); s.add_argument("statut"); s.add_argument("--preuve")
    s = sub.add_parser("hypothese-ajouter")
    s.add_argument("--tache", required=True); s.add_argument("--bloquante", choices=("oui", "non"), required=True)
    s.add_argument("--question", required=True); s.add_argument("--hypothese", required=True)
    s.add_argument("--impact", required=True)
    s = sub.add_parser("lancer-phase"); s.add_argument("phase")
    s = sub.add_parser("valider-phase"); s.add_argument("phase")
    # « ID texte libre » : accepte un seul argument entre guillemets (cas des skills) ou plusieurs
    s = sub.add_parser("refuser-tache"); s.add_argument("texte", nargs="+")
    s = sub.add_parser("repondre-hypothese"); s.add_argument("texte", nargs="+")
    args = ap.parse_args()
    if args.cmd in ("refuser-tache", "repondre-hypothese"):
        morceaux = " ".join(args.texte).strip().split(maxsplit=1)
        if len(morceaux) < 2:
            print("ERREUR : attendu « <identifiant> <texte> ».", file=sys.stderr)
            return 1
        args.id = morceaux[0]
        args.motif = args.reponse = morceaux[1]
    fonction = globals()["cmd_" + args.cmd.replace("-", "_")]
    try:
        sortie = fonction(args)
    except ErreurBacklog as e:
        print(f"ERREUR : {e}", file=sys.stderr)
        return 1
    if sortie:
        print(sortie)
    return 0


if __name__ == "__main__":
    sys.exit(main())
