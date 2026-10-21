#!/usr/bin/env python3
"""Hook PreToolUse : garde-fous du mode autonome.

1. Les fichiers de pilotage (BACKLOG.md, HYPOTHESES.md) ne s'écrivent que via scripts/backlog.py.
2. Les commandes réservées à l'utilisateur (lancer / valider une phase, refuser une tâche,
   répondre à une hypothèse) sont interdites à Claude.
3. Aucune écriture hors du projet (sauf ../Documentation).
4. Aucune action sortante ou destructrice : git push, reset --hard, rm -rf large, DROP DATABASE.
"""
import json
import os
import re
import sys
from pathlib import Path

PROJET = Path(os.environ.get("CLAUDE_PROJECT_DIR", Path(__file__).resolve().parents[2])).resolve()
RACINES_AUTORISEES = [PROJET, (PROJET.parent / "Documentation").resolve()]
FICHIERS_PILOTAGE = ("BACKLOG.md", "HYPOTHESES.md", "BACKLOG.xlsx")
COMMANDES_UTILISATEUR = ("lancer-phase", "valider-phase", "refuser-tache", "repondre-hypothese")
INTERDITS_SHELL = [
    (r"\bgit\s+push\b", "Le push vers GitHub est fait par l'utilisateur après validation de phase."),
    (r"\bgit\s+reset\s+--hard\b", "reset --hard détruit du travail : utilise git revert ou git restore ciblé."),
    (r"\bgit\s+clean\s+-[a-z]*f", "git clean -f supprime des fichiers non suivis."),
    (r"\brm\s+-[a-z]*r[a-z]*f?\s+(/|~|\.\.|\*|\"?C:)", "Suppression récursive hors périmètre."),
    (r"Remove-Item\b.*-Recurse.*(C:\\\\?$|\.\.|\\\*)", "Suppression récursive hors périmètre."),
    (r"\bDROP\s+DATABASE\b", "Suppression de base interdite (utilise l'instance de test jetable)."),
    (r"--no-verify\b", "Les hooks git ne doivent pas être contournés."),
]


def refuser(raison: str) -> None:
    print(json.dumps({"hookSpecificOutput": {
        "hookEventName": "PreToolUse",
        "permissionDecision": "deny",
        "permissionDecisionReason": raison}}, ensure_ascii=False))
    sys.exit(0)


def chemin_autorise(chemin: str) -> bool:
    p = Path(chemin)
    if not p.is_absolute():
        p = PROJET / p
    p = p.resolve()
    temp = Path(os.environ.get("TEMP", os.environ.get("TMP", "/tmp"))).resolve()
    return any(p == r or r in p.parents for r in RACINES_AUTORISEES + [temp])


def main() -> None:
    sys.stdout.reconfigure(encoding="utf-8")
    try:
        entree = json.loads(sys.stdin.buffer.read().decode("utf-8") or "{}")
    except json.JSONDecodeError:
        return
    outil = entree.get("tool_name", "")
    params = entree.get("tool_input", {}) or {}

    if outil in ("Edit", "Write", "MultiEdit", "NotebookEdit"):
        chemin = params.get("file_path") or params.get("notebook_path") or ""
        if Path(chemin).name in FICHIERS_PILOTAGE:
            refuser(f"{Path(chemin).name} ne se modifie que via `python scripts/backlog.py` "
                    "(tache, hypothese-ajouter). Les validations sont réservées à l'utilisateur.")
        if chemin and not chemin_autorise(chemin):
            refuser(f"Écriture hors du projet interdite : {chemin}")
        return

    if outil in ("Bash", "PowerShell"):
        cmd = params.get("command", "")
        if "backlog.py" in cmd:
            for c in COMMANDES_UTILISATEUR:
                if re.search(rf"backlog\.py\S*\s+{c}\b", cmd):
                    refuser(f"`{c}` est réservé à l'utilisateur (/{c}). Termine tes tâches puis demande-lui.")
        elif any(f in cmd for f in FICHIERS_PILOTAGE) and re.search(r"(>|sed\s+-i|Set-Content|Out-File|Add-Content|tee\b|mv\b|cp\b)", cmd):
            refuser("Les fichiers de pilotage ne s'écrivent que via scripts/backlog.py.")
        for motif, raison in INTERDITS_SHELL:
            if re.search(motif, cmd, re.IGNORECASE):
                refuser(raison)


if __name__ == "__main__":
    main()
