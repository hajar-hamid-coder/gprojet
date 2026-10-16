#!/usr/bin/env python3
"""Vérifie l'environnement de développement GestionProjet.

  python scripts/verifier-env.py

Affiche un tableau OK / MANQUANT / OPTIONNEL. Code de sortie 1 si un outil requis manque.
Requis : .NET SDK 10, Python >= 3.12 + openpyxl, PostgreSQL 18 (binaires), Git.
Optionnels : Node.js, Docker (requis à partir de P3), uv, graphify.
PostgreSQL : même localisation que scripts/pg-test.py (variable PG_BIN, sinon chemins par défaut, sinon PATH).
"""
import importlib.util
import os
import shutil
import subprocess
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")
RACINE = Path(__file__).resolve().parent.parent


def charger_pg_test():
    # Réutilise la localisation des binaires PostgreSQL définie par pg-test.py.
    spec = importlib.util.spec_from_file_location("pg_test", RACINE / "scripts" / "pg-test.py")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


PG_TEST = charger_pg_test()


def binaire_pg(nom: str) -> str | None:
    dossier = os.environ.get("PG_BIN", "").strip()
    if dossier:
        # PG_BIN explicite : seul ce dossier compte (pas de repli silencieux vers les chemins par défaut).
        for ext in ("", ".exe"):
            p = Path(dossier) / f"{nom}{ext}"
            if p.is_file():
                return str(p)
        return None
    try:
        return PG_TEST.binaire(nom)
    except SystemExit:
        return None


def version(cmd: list[str]) -> str | None:
    try:
        r = subprocess.run(cmd, capture_output=True, text=True, timeout=30, encoding="utf-8", errors="replace")
    except (OSError, subprocess.TimeoutExpired):
        return None
    return (r.stdout.strip() or r.stderr.strip()) if r.returncode == 0 else None


def outil(nom: str) -> str | None:
    chemin = shutil.which(nom)
    return version([chemin, "--version"]) if chemin else None


def controler() -> list[tuple[str, bool, bool, str]]:
    """Retourne (outil, requis, présent, détail)."""
    lignes = []

    dn = version([shutil.which("dotnet") or "dotnet", "--version"])
    lignes.append((".NET SDK 10", True, bool(dn) and dn.split(".")[0] == "10", dn or "introuvable"))

    lignes.append(("Python >= 3.12", True, sys.version_info >= (3, 12), sys.version.split()[0]))
    lignes.append(("Module openpyxl", True, importlib.util.find_spec("openpyxl") is not None, "export Excel du backlog"))

    manquants = [n for n in ("psql", "pg_ctl", "initdb") if binaire_pg(n) is None]
    psql = binaire_pg("psql")
    pg_v = version([psql, "--version"]) if psql and not manquants else None
    pg_ok = not manquants and bool(pg_v) and " 18." in pg_v
    detail_pg = pg_v or ("binaires absents : " + ", ".join(manquants) if manquants else "version 18 requise")
    lignes.append(("PostgreSQL 18 (PG_BIN)", True, pg_ok, detail_pg))

    git = outil("git")
    lignes.append(("Git", True, git is not None, git or "introuvable"))

    node = outil("node")
    lignes.append(("Node.js (ponytail, motion)", False, node is not None, node or "absent, optionnel"))

    docker = outil("docker")
    lignes.append(("Docker", False, docker is not None, docker or "absent : requis à partir de P3"))

    uv = outil("uv")
    lignes.append(("uv", False, uv is not None, uv or "absent, optionnel (graphify)"))

    graphify = outil("graphify")
    lignes.append(("graphify", False, graphify is not None, graphify or "absent, optionnel"))
    return lignes


def main() -> int:
    lignes = controler()
    print(f"{'Outil':<28} {'Statut':<11} Détail")
    print("-" * 80)
    bloquant = False
    for nom, requis, present, detail in lignes:
        if present:
            statut = "OK"
        elif requis:
            statut = "MANQUANT"
            bloquant = True
        else:
            statut = "OPTIONNEL"
        print(f"{nom:<28} {statut:<11} {detail}")
    print()
    print("Échec : au moins un outil requis manque." if bloquant else "Environnement prêt pour les phases en cours.")
    return 1 if bloquant else 0


if __name__ == "__main__":
    sys.exit(main())
