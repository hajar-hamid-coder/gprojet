#!/usr/bin/env python3
"""Valide le schéma de référence sur l'instance PostgreSQL jetable (jamais la base gprojet).

Démarre pg-test (01 + 02), exécute database/03-tests-integrite.sql, vérifie chaque bloc
(« attendu ERREUR » doit produire une erreur, les autres aucune), puis arrête et supprime .pgtest/.
Code de sortie : 0 si tout est conforme, 1 sinon.
"""
import importlib.util
import re
import subprocess
import sys

from pathlib import Path

RACINE = Path(__file__).resolve().parent.parent
spec = importlib.util.spec_from_file_location("pg_test", RACINE / "scripts" / "pg-test.py")
pg = importlib.util.module_from_spec(spec)
spec.loader.exec_module(pg)

# Valeurs attendues en plus de l'absence d'erreur (blocs de lecture).
CONTENU = {"T7": "PRJ-2026-0001-T1", "T17": " 7"}


def verifier() -> int:
    pg.demarrer(False)
    sortie = subprocess.run(
        [pg.binaire("psql"), "-p", pg.PORT, "-U", "postgres", "-d", pg.BASE, "-f",
         str(RACINE / "database" / "03-tests-integrite.sql")],
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True, encoding="utf-8", errors="replace",
    ).stdout

    # Découpe sur les \echo '--- Tn ...' (psql les renvoie sans quotes).
    morceaux = re.split(r"^--- (T\d+) (.*)$", sortie, flags=re.M)
    preambule, blocs = morceaux[0], zip(morceaux[1::3], morceaux[2::3], morceaux[3::3])
    ok = 0
    total = 0
    if "ERROR" in preambule:
        print("KO  preambule (organigramme) : erreur inattendue\n" + preambule)
        total += 1
    for tid, titre, corps in blocs:
        total += 1
        attend_erreur = "attendu ERREUR" in titre
        a_erreur = "ERROR:" in corps
        conforme = a_erreur == attend_erreur and CONTENU.get(tid, "") in corps
        ok += conforme
        print(f"{'OK' if conforme else 'KO'}  {tid} {'(erreur attendue)' if attend_erreur else ''} {titre}")
        if not conforme:
            print(corps)
    print(f"{ok}/{total} vérifications conformes")
    return 0 if ok == total and total == 18 else 1


if __name__ == "__main__":
    sys.stdout.reconfigure(encoding="utf-8")
    try:
        code = verifier()
    except subprocess.CalledProcessError as e:
        print(f"ERREUR ({e.cmd[0]}) :\n{e.stdout}\n{e.stderr}", file=sys.stderr)
        code = 1
    finally:
        pg.arreter()
    sys.exit(code)
