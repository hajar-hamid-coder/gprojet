#!/usr/bin/env python3
"""Export du backlog vers Excel : backlog/BACKLOG.xlsx (vue générée, jamais éditée à la main).

Feuilles : « Ordre d'exécution » (toutes les tâches dans l'ordre global), une feuille par phase
(P0 … P19), « Hypothèses », « Légende ». Régénéré automatiquement par scripts/backlog.py
à chaque changement de statut ; manuellement : python scripts/backlog_excel.py
"""
from __future__ import annotations

import re
import sys
from datetime import datetime
from pathlib import Path

from openpyxl import Workbook
from openpyxl.formatting.rule import CellIsRule
from openpyxl.styles import Alignment, Border, Font, PatternFill, Side
from openpyxl.utils import get_column_letter
from openpyxl.worksheet.table import Table, TableStyleInfo

sys.path.insert(0, str(Path(__file__).resolve().parent))
import backlog as bl  # noqa: E402

SORTIE = bl.RACINE / "backlog" / "BACKLOG.xlsx"
POLICE = "Arial"

COULEURS_STATUT = {  # fond, texte
    "NON_FAIT": ("F2F2F2", "595959"), "NON_DEMARREE": ("F2F2F2", "595959"),
    "EN_COURS": ("DDEBF7", "1F4E78"),
    "A_VALIDER": ("FFF2CC", "7F6000"),
    "VALIDE": ("E2EFDA", "375623"), "VALIDEE": ("E2EFDA", "375623"),
    "BLOQUE": ("FCE4D6", "C00000"),
    "OUVERTE": ("FCE4D6", "C00000"), "REPONDUE": ("E2EFDA", "375623"),
}
COULEURS_MODELE = {"opus": ("E4DFEC", "5B2C6F"), "sonnet": ("DDEBF7", "1F4E78"), "haiku": ("E2EFDA", "375623")}

TITRE = Font(name=POLICE, size=14, bold=True, color="1F3864")
SOUS_TITRE = Font(name=POLICE, size=10, italic=True, color="595959")
ENTETE = Font(name=POLICE, size=10, bold=True, color="FFFFFF")
FOND_ENTETE = PatternFill("solid", fgColor="1F3864")
NORMAL = Font(name=POLICE, size=10)
GRAS = Font(name=POLICE, size=10, bold=True)
BORD = Border(*(Side(style="thin", color="BFBFBF"),) * 4)
HAUT_GAUCHE = Alignment(vertical="top", wrap_text=True)
CENTRE = Alignment(horizontal="center", vertical="top", wrap_text=True)


def nettoyer(texte: str) -> str:
    texte = texte.replace("**", "").replace("`", "")
    return re.sub(r"\s+\n", "\n", texte).strip()


def champ(bloc: list[str], nom: str) -> str:
    """Contenu d'un champ « **Nom** : … » jusqu'au champ suivant ou à la fin du bloc."""
    valeur, capture = [], False
    for ligne in bloc:
        if ligne.startswith(f"**{nom}**"):
            capture = True
            valeur.append(ligne.split(":", 1)[1] if ":" in ligne else "")
        elif capture and (ligne.startswith("**") or ligne.startswith("### ") or ligne.startswith("---")):
            break
        elif capture:
            valeur.append(ligne)
    return nettoyer("\n".join(valeur))


def details_phase(lignes: list[str], p: bl.Phase) -> str:
    fin = p.taches[0].debut if p.taches else p.ligne_meta + 5
    for ligne in lignes[p.ligne_meta + 1:fin]:
        if ligne.startswith("**Objectif**"):
            return nettoyer(ligne)
    return ""


def ordre_global(phases: list[bl.Phase]) -> list[tuple[bl.Phase, bl.Tache]]:
    """Phases dans l'ordre ; dans une phase, tri topologique stable (dépendances puis champ Ordre)."""
    resultat = []
    for p in phases:
        restantes = sorted(p.taches, key=lambda t: t.ordre)
        placees: set[str] = set()
        while restantes:
            pret = next((t for t in restantes if all(d in placees or not d.startswith(p.id + "-") for d in t.depend)), restantes[0])
            resultat.append((p, pret))
            placees.add(pret.id)
            restantes.remove(pret)
    return resultat


def compter(p: bl.Phase) -> tuple[int, int, int]:
    """(tâches, faites = A_VALIDER ou VALIDE, bloquées)."""
    return (len(p.taches), sum(t.statut in bl.TERMINES for t in p.taches), sum(t.statut == "BLOQUE" for t in p.taches))


def entete(ws, ligne: int, colonnes: list[tuple[str, int]]) -> None:
    for i, (nom, largeur) in enumerate(colonnes, start=1):
        c = ws.cell(row=ligne, column=i, value=nom)
        c.font, c.fill, c.alignment, c.border = ENTETE, FOND_ENTETE, CENTRE, BORD
        ws.column_dimensions[get_column_letter(i)].width = largeur


def colorer(ws, plage: str, couleurs: dict[str, tuple[str, str]]) -> None:
    for valeur, (fond, texte) in couleurs.items():
        ws.conditional_formatting.add(plage, CellIsRule(
            operator="equal", formula=[f'"{valeur}"'],
            fill=PatternFill("solid", fgColor=fond), font=Font(name=POLICE, size=10, bold=True, color=texte)))


def table(ws, nom: str, ref: str) -> None:
    t = Table(displayName=nom, ref=ref)
    t.tableStyleInfo = TableStyleInfo(name="TableStyleLight1", showRowStripes=True)
    ws.add_table(t)


def ecrire_ligne(ws, ligne: int, valeurs: list, centrees: set[int]) -> None:
    for i, v in enumerate(valeurs, start=1):
        c = ws.cell(row=ligne, column=i, value=v)
        c.font, c.border = NORMAL, BORD
        c.alignment = CENTRE if i in centrees else HAUT_GAUCHE


def nom_feuille(p: bl.Phase) -> str:
    return p.id  # « P0 » … « P19 » : court, sans espace (références de formules simples)


def feuille_ordre(wb: Workbook, phases, lignes_md, hyps) -> None:
    ws = wb.active
    ws.title = "Ordre d'exécution"
    ws["A1"] = "Backlog GestionProjet — ordre d'exécution"
    ws["A1"].font = TITRE
    ws["A2"] = (f"Généré le {datetime.now():%d/%m/%Y %H:%M} depuis backlog/BACKLOG.md. Ne pas modifier : "
                "les statuts changent via /lancer-phase, /valider-phase, /refuser-tache et scripts/backlog.py.")
    ws["A2"].font = SOUS_TITRE

    # Synthèse par phase. Valeurs calculées ici et non par formules : le classeur est régénéré à chaque
    # transition, et une formule sans valeur en cache s'affiche vide dans les visionneuses qui ne recalculent pas.
    entete(ws, 4, [("Phase", 8), ("Titre", 46), ("Statut phase", 15), ("Modèle conseillé", 14),
                   ("Tâches", 9), ("Faites", 9), ("Avancement", 12), ("Bloquées", 10)])
    totaux = [0, 0, 0]
    for i, p in enumerate(phases):
        r = 5 + i
        nb, faites, bloquees = compter(p)
        totaux = [totaux[0] + nb, totaux[1] + faites, totaux[2] + bloquees]
        ecrire_ligne(ws, r, [p.id, p.titre, p.statut, p.modele, nb, faites, faites / nb if nb else 0, bloquees],
                     {1, 3, 4, 5, 6, 7, 8})
        ws.cell(row=r, column=1).hyperlink = f"#'{nom_feuille(p)}'!A1"
        ws.cell(row=r, column=7).number_format = "0%"
    fin_synthese = 4 + len(phases)
    total = fin_synthese + 1
    ws.cell(row=total, column=2, value="Total").font = GRAS
    for col, val in ((5, totaux[0]), (6, totaux[1]), (7, totaux[1] / totaux[0] if totaux[0] else 0), (8, totaux[2])):
        c = ws.cell(row=total, column=col, value=val)
        c.font, c.alignment = GRAS, CENTRE
    ws.cell(row=total, column=7).number_format = "0%"
    colorer(ws, f"C5:C{fin_synthese}", COULEURS_STATUT)
    colorer(ws, f"D5:D{fin_synthese}", COULEURS_MODELE)

    # Séquence complète
    depart = total + 3
    ws.cell(row=depart - 1, column=1, value="Séquence d'exécution des tâches").font = Font(name=POLICE, size=12, bold=True, color="1F3864")
    colonnes = [("N°", 8), ("Phase", 46), ("Code", 15), ("Tâche", 14), ("Modèle", 9), ("Ordre", 9), ("Dépend de", 12),
                ("Statut", 10), ("Hypothèses", 14)]
    # largeurs déjà fixées par la synthèse pour A-H : on n'écrase que les en-têtes
    for i, (nom, _) in enumerate(colonnes, start=1):
        c = ws.cell(row=depart, column=i, value=nom)
        c.font, c.fill, c.alignment, c.border = ENTETE, FOND_ENTETE, CENTRE, BORD
    ws.column_dimensions["I"].width = 22
    ws.column_dimensions["H"].width = 13
    hyp_par_tache: dict[str, list[str]] = {}
    for h in hyps:
        hyp_par_tache.setdefault(h.tache, []).append(f"{h.id}{' ⛔' if h.bloquante and h.statut == 'OUVERTE' else ''}")
    r = depart
    for n, (p, t) in enumerate(ordre_global(phases), start=1):
        r += 1
        ecrire_ligne(ws, r, [n, f"{p.id} · {p.titre}", t.id, t.titre, t.modele, t.ordre,
                             ", ".join(t.depend) or "—", t.statut, ", ".join(hyp_par_tache.get(t.id, []))],
                     {1, 3, 5, 6, 7, 8})
        ws.cell(row=r, column=3).hyperlink = f"#'{nom_feuille(p)}'!A1"
    # Colonnes de la séquence : Phase (B) large, Tâche (D) large
    ws.column_dimensions["B"].width = 46
    ws.column_dimensions["D"].width = 58
    colorer(ws, f"E{depart + 1}:E{r}", COULEURS_MODELE)
    colorer(ws, f"H{depart + 1}:H{r}", COULEURS_STATUT)
    ws.auto_filter.ref = f"A{depart}:I{r}"
    ws.freeze_panes = "A5"


def feuille_phase(wb: Workbook, p: bl.Phase, lignes_md, hyps) -> None:
    ws = wb.create_sheet(nom_feuille(p))
    ws["A1"] = f"{p.id} · {p.titre}"
    ws["A1"].font = TITRE
    ws["A2"] = details_phase(lignes_md, p)
    ws["A2"].font = SOUS_TITRE
    ws["A2"].alignment = Alignment(wrap_text=True, vertical="top")
    ws.merge_cells("A2:J2")
    ws.row_dimensions[2].height = 32
    nb, faites, _ = compter(p)
    infos = [("Statut phase", p.statut), ("Modèle conseillé", p.modele), ("Avancement", faites / nb if nb else 0)]
    for i, (lib, val) in enumerate(infos):
        ws.cell(row=3 + i, column=1, value=lib).font = GRAS
        c = ws.cell(row=3 + i, column=2, value=val)
        c.font = GRAS
    ws["B5"].number_format = "0%"
    colorer(ws, "B3", COULEURS_STATUT)
    colorer(ws, "B4", COULEURS_MODELE)
    ws.cell(row=6, column=1, value="← Retour à l'ordre d'exécution").hyperlink = "#'Ordre d''exécution'!A1"
    ws.cell(row=6, column=1).font = Font(name=POLICE, size=10, color="0563C1", underline="single")

    entete(ws, 7, [("Ordre", 8), ("Code", 11), ("Tâche", 34), ("Modèle", 9), ("Dépend de", 16), ("Statut", 12),
                   ("Explication", 70), ("Test de validation", 60), ("Retours utilisateur", 30), ("Hypothèses liées", 34)])
    r = 7
    for t in sorted(p.taches, key=lambda x: x.ordre):
        r += 1
        bloc = lignes_md[t.debut:t.fin]
        retours = "\n".join(nettoyer(l.lstrip("> ")) for l in bloc if l.startswith("> **Retour utilisateur"))
        liees = "\n".join(f"{h.id} ({h.statut}{', bloquante' if h.bloquante else ''})" for h in hyps if h.tache == t.id)
        ecrire_ligne(ws, r, [t.ordre, t.id, t.titre, t.modele, ", ".join(t.depend) or "—", t.statut,
                             champ(bloc, "Explication"), champ(bloc, "Test de validation"), retours, liees],
                     {1, 2, 4, 5, 6})
    if r > 7:
        colorer(ws, f"D8:D{r}", COULEURS_MODELE)
        colorer(ws, f"F8:F{r}", COULEURS_STATUT)
        table(ws, f"Taches_{p.id}", f"A7:J{r}")
    ws.freeze_panes = "C8"


def feuille_hypotheses(wb: Workbook, hyps) -> None:
    ws = wb.create_sheet("Hypothèses")
    ws["A1"] = "Feuille des hypothèses"
    ws["A1"].font = TITRE
    ws["A2"] = "Réponse : /repondre-hypothese H-00x <réponse>. Une hypothèse bloquante ouverte met sa tâche en BLOQUE."
    ws["A2"].font = SOUS_TITRE
    entete(ws, 4, [("Code", 9), ("Tâche", 11), ("Statut", 12), ("Bloquante", 11), ("Date", 17), ("Question", 55),
                   ("Hypothèse par défaut", 55), ("Impact si fausse", 45), ("Réponse utilisateur", 45)])
    lignes = bl.lire(bl.HYPOTHESES)
    r = 4
    for h in hyps:
        r += 1
        bloc = lignes[h.debut:h.fin]
        m = bl.RE_META_HYP.match(lignes[h.ligne_meta])
        reponse = next((nettoyer(l.split(":", 1)[1]) for l in bloc if l.startswith("**Réponse utilisateur**")), "")
        ecrire_ligne(ws, r, [h.id, h.tache, h.statut, "oui" if h.bloquante else "non", m[3],
                             champ(bloc, "Question"), champ(bloc, "Hypothèse retenue par défaut"),
                             champ(bloc, "Impact si l'hypothèse est fausse"), reponse.replace("_en attente_", "en attente")],
                     {1, 2, 3, 4, 5})
    if r > 4:
        colorer(ws, f"C5:C{r}", COULEURS_STATUT)
        table(ws, "Hypotheses", f"A4:I{r}")
    ws.freeze_panes = "C5"


def feuille_legende(wb: Workbook) -> None:
    ws = wb.create_sheet("Légende")
    ws["A1"] = "Légende et règles"
    ws["A1"].font = TITRE
    entete(ws, 3, [("Élément", 22), ("Valeur", 16), ("Signification", 90)])
    lignes = [
        ("Statut de tâche", "NON_FAIT", "Pas commencée."),
        ("Statut de tâche", "EN_COURS", "En cours d'exécution par Claude (une seule à la fois)."),
        ("Statut de tâche", "A_VALIDER", "Terminée par Claude, test de validation réussi et rejoué par le vérificateur."),
        ("Statut de tâche", "VALIDE", "Validée par l'utilisateur avec la phase (/valider-phase)."),
        ("Statut de tâche", "BLOQUE", "En attente de la réponse à une hypothèse bloquante (/repondre-hypothese)."),
        ("Statut de phase", "NON_DEMARREE", "Pas lancée. Se lance avec /lancer-phase quand la précédente est VALIDEE."),
        ("Statut de phase", "EN_COURS", "Claude exécute les tâches de façon autonome."),
        ("Statut de phase", "A_VALIDER", "Toutes les tâches sont A_VALIDER : Claude attend votre validation."),
        ("Statut de phase", "VALIDEE", "Validée par l'utilisateur."),
        ("Modèle", "opus", "Tâche complexe : architecture, sécurité, algorithmes, générateur."),
        ("Modèle", "sonnet", "Tâche moyenne : CRUD, pages, services, intégrations."),
        ("Modèle", "haiku", "Tâche simple : configuration, données, documentation."),
        ("Ordre", "Ordre / Dépend de", "Une tâche ne démarre que si ses dépendances sont A_VALIDER ou VALIDE ; sinon l'ordre de la phase."),
        ("Fichier", "BACKLOG.xlsx", "Vue générée depuis backlog/BACKLOG.md à chaque changement. Toute modification manuelle est écrasée."),
    ]
    for i, l in enumerate(lignes, start=4):
        ecrire_ligne(ws, i, list(l), {2})
    colorer(ws, f"B4:B{3 + len(lignes)}", {**COULEURS_STATUT, **COULEURS_MODELE})


def exporter(sortie: Path = SORTIE) -> Path:
    lignes_md = bl.lire(bl.BACKLOG)
    phases = bl.charger_backlog(lignes_md)
    hyps = bl.charger_hypotheses(bl.lire(bl.HYPOTHESES))
    wb = Workbook()
    feuille_ordre(wb, phases, lignes_md, hyps)
    for p in phases:
        feuille_phase(wb, p, lignes_md, hyps)
    feuille_hypotheses(wb, hyps)
    feuille_legende(wb)
    wb.save(sortie)
    return sortie


def main() -> int:
    sys.stdout.reconfigure(encoding="utf-8")
    try:
        print(f"Excel généré : {exporter()}")
    except PermissionError:
        print("ERREUR : BACKLOG.xlsx est ouvert dans Excel ; fermez-le puis relancez.", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
