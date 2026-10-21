#!/usr/bin/env python3
"""Instance PostgreSQL 18 jetable pour les tests (aucun mot de passe, aucune donnée réelle).

  python scripts/pg-test.py demarrer [--vide]   initdb + start sur le port 55432, base gp_test
                                                (schéma + données de référence sauf --vide)
  python scripts/pg-test.py sql "SELECT 1"      exécute une requête sur gp_test
  python scripts/pg-test.py fichier chemin.sql  exécute un fichier SQL sur gp_test
  python scripts/pg-test.py arreter             stoppe et supprime l'instance
  python scripts/pg-test.py chaine              affiche la chaîne de connexion Npgsql

Binaires : variable PG_BIN, sinon C:\\Program Files\\PostgreSQL\\18\\bin, sinon le PATH.
"""
import os
import shutil
import subprocess
import sys
from pathlib import Path

RACINE = Path(__file__).resolve().parent.parent
DONNEES = RACINE / ".pgtest" / "data"
JOURNAL = RACINE / ".pgtest" / "pg.log"
PORT = os.environ.get("PG_TEST_PORT", "55432")
BASE = "gp_test"
CHAINE = f"Host=localhost;Port={PORT};Database={BASE};Username=postgres"


def binaire(nom: str) -> str:
    candidats = [os.environ.get("PG_BIN", ""), r"C:\Program Files\PostgreSQL\18\bin"]
    for dossier in candidats:
        for ext in ("", ".exe"):
            p = Path(dossier) / f"{nom}{ext}"
            if dossier and p.exists():
                return str(p)
    trouve = shutil.which(nom)
    if not trouve:
        sys.exit(f"ERREUR : {nom} introuvable (définis PG_BIN).")
    return trouve


def lancer(*args: str, verifier: bool = True) -> subprocess.CompletedProcess:
    return subprocess.run(args, check=verifier, capture_output=True, text=True, encoding="utf-8", errors="replace")


def psql(*args: str) -> subprocess.CompletedProcess:
    return lancer(binaire("psql"), "-p", PORT, "-U", "postgres", "-d", BASE, "-v", "ON_ERROR_STOP=1", *args)


def en_marche() -> bool:
    return DONNEES.exists() and lancer(binaire("pg_ctl"), "-D", str(DONNEES), "status", verifier=False).returncode == 0


def demarrer(vide: bool) -> None:
    if en_marche():
        print(f"Déjà démarrée. {CHAINE}")
        return
    if DONNEES.exists():
        shutil.rmtree(DONNEES)
    DONNEES.parent.mkdir(parents=True, exist_ok=True)
    lancer(binaire("initdb"), "-D", str(DONNEES), "-U", "postgres", "-A", "trust", "-E", "UTF8", "--locale=C")
    # Pas de capture ici : le processus postgres hériterait des tubes et bloquerait l'attente (Windows).
    subprocess.run([binaire("pg_ctl"), "-D", str(DONNEES), "-o", f"-p {PORT}", "-l", str(JOURNAL), "-w", "start"],
                   check=True, stdin=subprocess.DEVNULL, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    lancer(binaire("psql"), "-p", PORT, "-U", "postgres", "-d", "postgres", "-c", f"CREATE DATABASE {BASE};")
    if not vide:
        for f in ("01-schema.sql", "02-donnees-reference.sql"):
            psql("-q", "-f", str(RACINE / "database" / f))
    print(f"Instance de test prête ({'vide' if vide else 'schéma + référence'}). {CHAINE}")


def arreter() -> None:
    if en_marche():
        lancer(binaire("pg_ctl"), "-D", str(DONNEES), "-m", "fast", "-w", "stop")
    shutil.rmtree(DONNEES.parent, ignore_errors=True)
    print("Instance de test arrêtée et supprimée.")


def main() -> int:
    sys.stdout.reconfigure(encoding="utf-8")
    if len(sys.argv) < 2:
        print(__doc__)
        return 1
    cmd, reste = sys.argv[1], sys.argv[2:]
    try:
        if cmd == "demarrer":
            demarrer("--vide" in reste)
        elif cmd == "arreter":
            arreter()
        elif cmd == "chaine":
            print(CHAINE)
        elif cmd == "sql":
            print(psql("-c", " ".join(reste)).stdout)
        elif cmd == "fichier":
            print(psql("-f", reste[0]).stdout)
        else:
            print(__doc__)
            return 1
    except subprocess.CalledProcessError as e:
        print(f"ERREUR ({' '.join(e.cmd[:1])}) :\n{e.stdout}\n{e.stderr}", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
