# Déploiement local et pipeline d'intégration

Ce document décrit le lancement de la plateforme en local avec Docker Compose, les variables utilisées, le pipeline GitHub Actions et les problèmes courants. Toutes les commandes sont à lancer depuis la racine du dépôt.

## Prérequis

- Docker Desktop avec Docker Compose v2 (`docker compose version`). Installation via `scripts\installer-outils.ps1` (étape Docker Desktop).
- Git.
- Python 3 pour les scripts du dépôt (ex. `python scripts/verifier-ci.py`, qui demande PyYAML : `pip install pyyaml`).
- Pour le développement hors conteneur : .NET SDK 10 et PostgreSQL 18 (contrôle par `python scripts/verifier-env.py`).

## Variables

### Fichier `deploy/.env` (Docker Compose)

Copier le modèle, puis remplacer les valeurs :

```powershell
copy deploy\.env.example deploy\.env
```

| Variable | Obligatoire | Valeur par défaut | Rôle |
|---|---|---|---|
| `POSTGRES_PASSWORD` | oui | aucune (`CHANGER_MOI` dans le modèle) | Mot de passe de l'utilisateur `gprojet`. Compose refuse de démarrer si la variable est absente. |
| `POSTGRES_HOST_PORT` | non | `55433` | Port hôte de PostgreSQL. `5432` (base de dev) et `55432` (instance de test `pg-test`) sont réservés. |
| `APP_HOST_PORT` | non | `8080` | Port hôte de l'application. |
| `KEYCLOAK_ADMIN_PASSWORD` | seulement pour le profil `keycloak` | vide | Mot de passe administrateur Keycloak (profil `keycloak`, port hôte 8081). |

Le fichier `deploy/.env` ne doit jamais être commité (il est ignoré par Git). Le modèle `deploy/.env.example` ne contient que des valeurs de substitution.

### Variable d'exécution `ConnectionStrings__Gp`

L'application et le bundle de migrations lisent la chaîne de connexion dans `ConnectionStrings__Gp`. Dans Compose, elle est construite par le fichier `deploy/docker-compose.yml`. Hors Compose (ex. `dotnet run`), la définir dans l'environnement local, jamais dans un fichier suivi par Git. Ne pas afficher ni recopier sa valeur réelle.

### Variable de test `GP_TEST_CONNECTION`

Chaîne Npgsql d'un serveur PostgreSQL pour les tests d'intégration. Si elle est absente, les tests utilisent `python scripts/pg-test.py demarrer`. Détails dans `CONTRIBUTING.md`.

### Secrets

Aucun secret n'est à créer dans GitHub aujourd'hui. Le job de publication s'authentifie à ghcr.io avec le `GITHUB_TOKEN` fourni automatiquement par GitHub Actions. Aucun mot de passe réel ne doit figurer dans le dépôt, dans `deploy/.env.example` ou dans la documentation.

## Démarrage

Vérifier la configuration, puis démarrer la base, les migrations et l'application :

```powershell
docker compose -f deploy/docker-compose.yml config -q
docker compose -f deploy/docker-compose.yml up -d --wait
```

- `config -q` valide le fichier et les variables sans rien lancer.
- `up -d --wait` construit les images, démarre PostgreSQL 18, attend son healthcheck, exécute le service `migrations` (qui doit se terminer avec succès), puis démarre `app`.
- L'application répond sur `http://localhost:8080` (ou `APP_HOST_PORT`). Le point de contrôle `/health` sert au HEALTHCHECK de l'image.

Arrêter sans effacer les données :

```powershell
docker compose -f deploy/docker-compose.yml down
```

Profil Keycloak (optionnel, nécessite `KEYCLOAK_ADMIN_PASSWORD`) :

```powershell
docker compose -f deploy/docker-compose.yml --profile keycloak up -d
```

## Migrations

Les migrations ne sont pas lancées par l'application. Le service `migrations` exécute le bundle EF Core (cible `migrations` du `deploy/docker/Dockerfile`, construit avec `dotnet ef migrations bundle`) une fois avant `app`, puis s'arrête (`restart: "no"`). Pour les relancer après une modification du schéma :

```powershell
docker compose -f deploy/docker-compose.yml up -d --build migrations
```

## Pipeline

Le workflow `.github/workflows/ci.yml` se déclenche sur chaque push, sur chaque pull request, sur les tags `v*` et manuellement (`workflow_dispatch`).

| Job | Quand il tourne | Rôle |
|---|---|---|
| `build` | Toujours | Restauration puis compilation de `Gprojet.slnx` en Release. |
| `test` | Après `build` | Tests avec couverture (`dotnet test`), PostgreSQL 18 de service ; rapports TRX publiés en artefact. |
| `qualite` | Toujours | Vérification du formatage : `dotnet format Gprojet.slnx --verify-no-changes`. |
| `securite` | Toujours | Dependency review (pull request), CodeQL, construction de l'image `final` puis scan Trivy. Trivy est **bloquant** sur les vulnérabilités CRITICAL et HIGH (corrigeables). |
| `paquet` | Après les quatre jobs précédents, sur `main` ou un tag `v*` | Construction et publication de l'image sur ghcr.io. |
| `deploiement` | Sur `workflow_dispatch` seulement, si `paquet` réussit | Environnement `production`. Le déploiement n'est pas encore branché (phase P13). |

Publication sur ghcr.io : `ghcr.io/hajar-hamid-coder/gprojet`, avec les tags suivants :

- `sha-<commit>` pour chaque build ;
- la version (`v1.2.3` par exemple) pour un tag Git `v*` ;
- `latest` uniquement sur `main`.

Environnement `production` : à créer dans GitHub, Settings → Environments → New environment → `production`, avec des relecteurs requis (Required reviewers). Le job `deploiement` ne démarre qu'après leur approbation.

Vérifier localement la conformité du workflow :

```powershell
python scripts/verifier-ci.py
```

## Dépannage

**Port 8080 déjà occupé** : définir `APP_HOST_PORT` dans `deploy/.env` (ex. `APP_HOST_PORT=8090`), puis relancer `up -d --wait`.

**Port PostgreSQL déjà occupé** (`5432` ou `55432`) : définir `POSTGRES_HOST_PORT=55433` dans `deploy/.env` (valeur par défaut du modèle).

**`POSTGRES_PASSWORD` manquant** : Compose affiche l'erreur « definir POSTGRES_PASSWORD dans deploy/.env ». Créer `deploy/.env` à partir de `deploy/.env.example`, ou définir la variable dans le shell.

**Échec du service `migrations`** : consulter ses journaux.

```powershell
docker compose -f deploy/docker-compose.yml logs migrations
```

Le service `app` ne démarre pas tant que les migrations ne se sont pas terminées avec succès.

**Repartir de zéro** : supprimer les conteneurs et le volume `pgdata` (toutes les données locales sont perdues) :

```powershell
docker compose -f deploy/docker-compose.yml down -v
```

**Git Bash et chemins de conteneur** : Git Bash convertit les chemins commençant par `/` (ex. `/app`) en chemins Windows. Pour passer un chemin de conteneur depuis Git Bash, préfixer la commande :

```bash
MSYS_NO_PATHCONV=1 docker compose -f deploy/docker-compose.yml exec app ls /app
```

**Test de l'environnement** : `python scripts/verifier-env.py` affiche les outils manquants (Docker est requis à partir de P3).
