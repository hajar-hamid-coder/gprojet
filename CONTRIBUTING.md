# Contribuer à GestionProjet

Les règles complètes (périmètre, conventions, interdits) sont dans [CLAUDE.md](CLAUDE.md). Ce guide résume ce qui s'applique au quotidien. En cas de doute, CLAUDE.md prime.

## Branches

- Une branche par phase : `phase/Pn-<slug>` (ex. `phase/P0-initialisation`), créée depuis la branche de la phase précédente validée.
- Pas de commit direct sur la branche par défaut.
- Pas de `git push` avant la validation de la phase par l'utilisateur.

## Commits

- Un commit par tâche, au format `Pn-Tnn: <titre>` (ex. `P0-T05: Modèles GitHub et guide de contribution`).
- Le commit ne contient que les fichiers de la tâche. Les fichiers de pilotage `backlog/*.md` sont mis à jour par l'orchestrateur avec `python scripts/backlog.py`, jamais à la main.
- Pas de `--no-verify` ni de contournement de signature.

## Revue

- Chaque pull request GitHub utilise le modèle `.github/pull_request_template.md` : résumé, tâches, preuves, hypothèses, checklist.
- Les anomalies et demandes utilisent `.github/ISSUE_TEMPLATE/Anomalie.md` et `Demande.md`.
- Toute tâche codée par un sous-agent sonnet ou opus passe par une relecture (`ponytail-review`) avant commit.
- Une tâche n'est marquée `A_VALIDER` qu'après exécution de son test de validation.

## Architecture

- Deux projets applicatifs : `src/Gprojet-Backend` (API et BFF, sert aussi le front) et `src/Gprojet-Front` (Blazor WebAssembly).
- Couches du backend : `Web → Orchestration → Services → Repositories → Entities`.
- Les DTO ne descendent jamais sous l'Orchestration ; les entités ne remontent jamais au-dessus.
- Les contrôleurs n'appellent que des orchestrateurs. Toute requête sur les projets passe par le périmètre (`PerimetreService`).
- Le code généré vit dans des fichiers `*.g.cs` ; le code manuel n'y va jamais.
- Les tests NetArchTest (`tests/Gprojet-Backend.Tests/Architecture`) gardent ces frontières : ne jamais les affaiblir.
- Charte graphique obligatoire : `design/charte-hcp/`. Pas de MudBlazor, aucune ressource chargée depuis Internet.

## Tests

- Frameworks : xUnit et FluentAssertions ; bUnit pour les composants ; NetArchTest pour les couches.
- Tests et PostgreSQL : l'instance jetable uniquement, jamais la base `gprojet`.

```bash
python scripts/pg-test.py demarrer     # instance jetable, port 55432
python scripts/pg-test.py arreter      # à l'arrêt
python scripts/tester-schema.py        # valide database/01 à 03 sur l'instance jetable
python scripts/verifier-env.py         # contrôle de l'environnement local
dotnet build                           # sans avertissement nouveau
```

## Secrets

- Aucun secret, mot de passe ou chaîne de connexion dans le dépôt ni dans un fichier suivi.
- La connexion de développement passe par la variable d'environnement locale, non versionnée.

## Intégration continue

Le dépôt est hébergé sur GitHub (public) et la CI tourne avec GitHub Actions. GitLab reste une fonctionnalité de la plateforme (phases P12 et P13), pas l'outil de dépôt.
