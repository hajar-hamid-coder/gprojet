#!/usr/bin/env python3
"""Vérifie .github/workflows/ci.yml (structure, dépendances, service PostgreSQL, secrets, épinglage) et les modèles .github/.

  python scripts/verifier-ci.py              code 0 si tout est conforme, sinon liste des écarts et code 1
  python scripts/verifier-ci.py --auto-test  contrôle négatif : un YAML piégé doit être rejeté
Nécessite PyYAML (pip install pyyaml).
"""
import copy
import re
import sys
from pathlib import Path

try:
    import yaml
except ImportError:
    sys.exit("PyYAML absent : pip install pyyaml")

RACINE = Path(__file__).resolve().parent.parent
JOBS = {
    "build": set(), "test": {"build"}, "qualite": set(), "securite": set(),
    "paquet": {"build", "test", "qualite", "securite"}, "deploiement": {"paquet"},
}
# Liste blanche : seul le mot de passe de la base éphémère du service postgres du runner (pas un secret réel).
MDP_TOLERE = "postgres"
SHA = re.compile(r"^[0-9a-f]{40}$")
VERSION = re.compile(r"^v?\d+\.\d+(\.\d+)?$")
SECRET = re.compile(r"(password|pwd|token|secret|api[_-]?key)\s*[=:]\s*(?!\$\{\{)(\S+)", re.I)
CLE_PRIVEE = re.compile(r"-----BEGIN [A-Z ]*PRIVATE KEY|ghp_[A-Za-z0-9]{20,}|AKIA[0-9A-Z]{16}")


def parcourir(n):
    if isinstance(n, dict):
        for k, v in n.items():
            if isinstance(v, (str, int)):
                yield f"{k}: {v}"  # la clé compte (POSTGRES_PASSWORD: ...)
            else:
                yield from parcourir(v)
    elif isinstance(n, list):
        for v in n:
            yield from parcourir(v)
    elif n is not None:
        yield n


def verifier(texte: str) -> list[str]:
    ecarts: list[str] = []
    try:
        wf = yaml.safe_load(texte)
    except yaml.YAMLError as e:
        return [f"YAML invalide : {e}"]
    jobs = wf.get("jobs", {})
    for nom, needs in JOBS.items():
        if nom not in jobs:
            ecarts.append(f"job manquant : {nom}")
            continue
        n = jobs[nom].get("needs", [])
        n = {n} if isinstance(n, str) else set(n)
        if n != needs:
            ecarts.append(f"job {nom} : needs {sorted(n)} au lieu de {sorted(needs)}")
    for nom in jobs.keys() - JOBS.keys():
        ecarts.append(f"job inattendu : {nom}")

    declencheurs = wf.get("on", wf.get(True, {}))  # YAML 1.1 lit « on » comme True
    for d in ("push", "pull_request", "workflow_dispatch"):
        if d not in declencheurs:
            ecarts.append(f"déclencheur manquant : {d}")
    if wf.get("permissions") != {"contents": "read"}:
        ecarts.append("permissions du workflow : contents: read exigé, rien d'autre")

    pg = jobs.get("test", {}).get("services", {}).get("postgres", {})
    if pg.get("image") != "postgres:18":
        ecarts.append("job test : service postgres:18 manquant")
    if "GP_TEST_CONNECTION" not in jobs.get("test", {}).get("env", {}):
        ecarts.append("job test : GP_TEST_CONNECTION manquante")

    paquet = jobs.get("paquet", {})
    if paquet.get("permissions", {}).get("packages") != "write":
        ecarts.append("job paquet : permissions packages: write manquante")
    cond = paquet.get("if", "")
    if "refs/heads/main" not in cond or "refs/tags/" not in cond:
        ecarts.append("job paquet : condition main/tags manquante")
    if "environment" not in jobs.get("deploiement", {}):
        ecarts.append("job deploiement : environment protégé manquant")
    if "workflow_dispatch" not in jobs.get("deploiement", {}).get("if", ""):
        ecarts.append("job deploiement : doit être limité à workflow_dispatch")

    # Épinglage : SHA complet (idéal) ou version exacte ; jamais @main / @master / @v4 flottant.
    for nom, job in jobs.items():
        for etape in job.get("steps", []):
            u = etape.get("uses")
            if not u:
                continue
            ref = u.split("@", 1)[1] if "@" in u else ""
            if not (SHA.match(ref) or VERSION.match(ref) and ref.count(".") >= 1):
                ecarts.append(f"job {nom} : action non épinglée : {u}")

    # Secrets en clair (hors expressions ${{ }} et mot de passe de la liste blanche).
    for valeur in parcourir(wf):
        s = str(valeur)
        if CLE_PRIVEE.search(s):
            ecarts.append("secret en clair : motif de clé/jeton détecté")
        for m in SECRET.finditer(s):
            if m.group(1).lower() == "password" and m.group(2).strip(";\"'") == MDP_TOLERE:
                continue
            ecarts.append(f"secret en clair probable : {m.group(0)[:40]}")
    return ecarts


def modeles() -> list[str]:
    g = RACINE / ".github"
    ecarts = []
    if not (g / "pull_request_template.md").is_file():
        ecarts.append(".github/pull_request_template.md absent")
    if not list((g / "ISSUE_TEMPLATE").glob("*.md")):
        ecarts.append(".github/ISSUE_TEMPLATE/*.md absent")
    return ecarts


def auto_test(texte: str) -> int:
    pieges = {
        "action flottante": texte.replace("actions/checkout@11bd71901bbe5b1630ceea73d27597364c9af683", "actions/checkout@main", 1),
        "mot de passe en clair": texte.replace("POSTGRES_PASSWORD: postgres", "POSTGRES_PASSWORD: Hunter2xyz", 1),
        "service postgres retiré": texte.replace("image: postgres:18", "image: postgres:16", 1),
    }
    wf = yaml.safe_load(texte)
    wf["jobs"]["paquet"]["needs"] = ["build"]
    pieges["needs altéré"] = yaml.safe_dump(wf)
    ok = not verifier(texte)
    for nom, t in pieges.items():
        if not verifier(t):
            print(f"AUTO-TEST KO : piège non détecté ({nom})")
            ok = False
    print("AUTO-TEST OK : 4 pièges rejetés, original conforme" if ok else "AUTO-TEST KO")
    return 0 if ok else 1


def main() -> int:
    texte = (RACINE / ".github" / "workflows" / "ci.yml").read_text(encoding="utf-8")
    if "--auto-test" in sys.argv:
        return auto_test(texte)
    ecarts = verifier(texte) + modeles()
    for e in ecarts:
        print("ECART :", e)
    print(f"{len(ecarts)} écart(s)" if ecarts else "CI conforme : 6 jobs, dépendances, postgres:18, aucun secret, actions épinglées, modèles présents")
    return 1 if ecarts else 0


if __name__ == "__main__":
    sys.exit(main())
