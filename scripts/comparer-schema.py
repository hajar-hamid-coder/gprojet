#!/usr/bin/env python3
"""Compare le schéma produit par les migrations EF Core au schéma de référence (instance jetable uniquement).

Sur l'instance pg-test (port 55432, jamais la base gprojet), crée deux bases :
  * ref_sql : database/01-schema.sql puis 02-donnees-reference.sql
  * ref_ef  : dotnet ef database update (ConnectionStrings__Gp surchargée vers ref_ef)
puis compare via pg_catalog : schémas, extensions, relations, colonnes (type, nullabilité, défaut, identité),
contraintes, index, triggers, fonctions, et le nombre de lignes de chaque table (données de référence).
Exclusion explicite : la table __EFMigrationsHistory (et sa clé primaire), propre à EF.

  python scripts/comparer-schema.py              « 0 écart » et code 0, sinon liste des écarts et code 1
  python scripts/comparer-schema.py --auto-test  contrôle négatif : altère ref_ef et exige que les écarts soient vus
"""
import importlib.util
import os
import subprocess
import sys
from pathlib import Path

RACINE = Path(__file__).resolve().parent.parent
spec = importlib.util.spec_from_file_location("pg_test", RACINE / "scripts" / "pg-test.py")
pg = importlib.util.module_from_spec(spec)
spec.loader.exec_module(pg)

EXCLUSION = "__EFMigrationsHistory"
SEP = "\x1f"
# Objets propres à l'application : hors catalogues système et hors membres d'extension (ltree).
NS = "n.nspname NOT IN ('pg_catalog','information_schema','pg_toast') AND n.nspname NOT LIKE 'pg_temp%'"
HORS_EXT = "NOT EXISTS (SELECT 1 FROM pg_depend d WHERE d.objid = {oid} AND d.deptype = 'e')"
REL = f"{NS} AND c.relname <> '{EXCLUSION}' AND " + HORS_EXT.format(oid="c.oid")

REQUETES = {
    "schéma": f"SELECT n.nspname FROM pg_namespace n WHERE {NS} AND n.nspname NOT LIKE 'pg_toast%'",
    "extension": "SELECT e.extname || ' @' || n.nspname FROM pg_extension e JOIN pg_namespace n ON n.oid = e.extnamespace",
    "relation": f"""SELECT n.nspname||'.'||c.relname, c.relkind, c.relpersistence FROM pg_class c
                    JOIN pg_namespace n ON n.oid = c.relnamespace WHERE {REL} AND c.relkind IN ('r','p','v','m','S','f')""",
    "colonne": f"""SELECT n.nspname||'.'||c.relname||'.'||a.attname, format_type(a.atttypid, a.atttypmod),
                          CASE WHEN a.attnotnull THEN 'NOT NULL' ELSE 'NULL' END,
                          coalesce(pg_get_expr(ad.adbin, ad.adrelid), '<sans defaut>'), a.attidentity, a.attgenerated,
                          coalesce(co.collname, '')
                   FROM pg_attribute a JOIN pg_class c ON c.oid = a.attrelid JOIN pg_namespace n ON n.oid = c.relnamespace
                   LEFT JOIN pg_attrdef ad ON ad.adrelid = a.attrelid AND ad.adnum = a.attnum
                   LEFT JOIN pg_collation co ON co.oid = a.attcollation AND a.attcollation <> 100
                   WHERE {REL} AND c.relkind IN ('r','p','v','m','f') AND a.attnum > 0 AND NOT a.attisdropped""",
    "contrainte": f"""SELECT n.nspname||'.'||c.relname||' '||k.conname, k.contype, pg_get_constraintdef(k.oid),
                             k.condeferrable, k.condeferred
                      FROM pg_constraint k JOIN pg_class c ON c.oid = k.conrelid JOIN pg_namespace n ON n.oid = c.relnamespace
                      WHERE {REL}""",
    "index": f"""SELECT n.nspname||'.'||i.relname, pg_get_indexdef(i.oid)
                 FROM pg_index x JOIN pg_class i ON i.oid = x.indexrelid JOIN pg_class c ON c.oid = x.indrelid
                 JOIN pg_namespace n ON n.oid = i.relnamespace WHERE {REL}""",
    "trigger": f"""SELECT n.nspname||'.'||c.relname||' '||t.tgname, pg_get_triggerdef(t.oid), t.tgenabled
                   FROM pg_trigger t JOIN pg_class c ON c.oid = t.tgrelid JOIN pg_namespace n ON n.oid = c.relnamespace
                   WHERE {REL} AND NOT t.tgisinternal""",
    "fonction": f"""SELECT n.nspname||'.'||p.proname||'('||pg_get_function_identity_arguments(p.oid)||')',
                           regexp_replace(pg_get_functiondef(p.oid), '\\s+', ' ', 'g')
                    FROM pg_proc p JOIN pg_namespace n ON n.oid = p.pronamespace
                    WHERE {NS} AND {HORS_EXT.format(oid='p.oid')}""",
    # Données : nombre de lignes de chaque table (01 + 02 d'un côté, migration de l'autre).
    "lignes": f"""SELECT n.nspname||'.'||c.relname,
                         (xpath('/row/n/text()', query_to_xml(format('SELECT count(*) AS n FROM %I.%I', n.nspname, c.relname),
                                false, true, '')))[1]::text
                  FROM pg_class c JOIN pg_namespace n ON n.oid = c.relnamespace WHERE {REL} AND c.relkind IN ('r','p')""",
}


def psql(base: str, *args: str) -> str:
    return pg.lancer(pg.binaire("psql"), "-p", pg.PORT, "-U", "postgres", "-d", base, "-v", "ON_ERROR_STOP=1",
                     "-X", "-q", *args).stdout


def creer_bases() -> None:
    pg.demarrer(True)  # instance vide (base gp_test inutilisée)
    for base in ("ref_sql", "ref_ef"):
        psql("postgres", "-c", f"CREATE DATABASE {base};")
    for f in ("01-schema.sql", "02-donnees-reference.sql"):
        psql("ref_sql", "-f", str(RACINE / "database" / f))

    env = os.environ.copy()
    env["ConnectionStrings__Gp"] = f"Host=localhost;Port={pg.PORT};Username=postgres;Database=ref_ef"
    r = subprocess.run(["dotnet", "tool", "run", "dotnet-ef", "database", "update", "--project", "src/Gprojet-Backend"],
                       cwd=RACINE, env=env, capture_output=True, text=True, encoding="utf-8", errors="replace")
    if r.returncode != 0:
        print(r.stdout[-4000:], r.stderr[-4000:], sep="\n", file=sys.stderr)
        raise SystemExit("ERREUR : dotnet ef database update a échoué sur ref_ef.")


def photo(base: str) -> dict[str, dict[str, str]]:
    resultat = {}
    for categorie, requete in REQUETES.items():
        lignes = psql(base, "-A", "-t", "-F", SEP, "-c", requete).splitlines()
        cles = {}
        for ligne in filter(None, lignes):
            cle, _, valeur = ligne.partition(SEP)
            cles[cle] = valeur.replace(SEP, " | ")
        resultat[categorie] = cles
    return resultat


def comparer() -> list[str]:
    a, b = photo("ref_sql"), photo("ref_ef")
    ecarts = []
    for categorie in REQUETES:
        ra, rb = a[categorie], b[categorie]
        for cle in sorted(ra.keys() | rb.keys()):
            if cle not in rb:
                ecarts.append(f"[{categorie}] {cle} : seulement dans 01-schema.sql ({ra[cle]})")
            elif cle not in ra:
                ecarts.append(f"[{categorie}] {cle} : seulement dans la migration ({rb[cle]})")
            elif ra[cle] != rb[cle]:
                ecarts.append(f"[{categorie}] {cle} :\n    01-schema.sql : {ra[cle]}\n    migration     : {rb[cle]}")
    taille = {c: len(a[c]) for c in REQUETES}
    print("Objets comparés (référence) : " + ", ".join(f"{c}={n}" for c, n in taille.items()))
    print(f"Exclusion explicite : {EXCLUSION}")
    return ecarts


def main() -> int:
    creer_bases()
    if "--auto-test" in sys.argv:
        # Trois altérations de nature différente : type de colonne, index supprimé, défaut modifié.
        psql("ref_ef", "-c", "ALTER TABLE org.agent ALTER COLUMN telephone TYPE varchar(31);"
                             "DROP INDEX org.ix_agent_fonction;"
                             "ALTER TABLE org.agent ALTER COLUMN capacite_hebdo_h SET DEFAULT 39;")
        ecarts = comparer()
        print("\n".join(ecarts))
        attendus = ("org.agent.telephone", "org.ix_agent_fonction", "org.agent.capacite_hebdo_h")
        vus = all(any(cle in e for e in ecarts) for cle in attendus)
        print(f"Contrôle négatif : {len(ecarts)} écart(s) détecté(s), altérations attendues "
              f"{'toutes détectées' if vus else 'NON détectées'}")
        return 0 if vus and len(ecarts) == len(attendus) else 1
    ecarts = comparer()
    for e in ecarts:
        print(e)
    print(f"{len(ecarts)} écart{'s' if len(ecarts) > 1 else ''}")
    return 1 if ecarts else 0


if __name__ == "__main__":
    sys.stdout.reconfigure(encoding="utf-8")
    try:
        code = main()
    except subprocess.CalledProcessError as e:
        print(f"ERREUR ({e.cmd[0]}) :\n{e.stdout}\n{e.stderr}", file=sys.stderr)
        code = 1
    finally:
        pg.arreter()
    sys.exit(code)
