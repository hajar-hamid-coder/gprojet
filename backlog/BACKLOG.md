# BACKLOG — GestionProjet

> Source de vérité du pilotage. **Statuts modifiés uniquement par `python scripts/backlog.py`** (Claude) ou par les commandes utilisateur `/lancer-phase`, `/valider-phase`, `/refuser-tache`, `/repondre-hypothese`.

## Mode d'emploi

| Élément | Règle |
|---|---|
| Ordre des phases | Une phase ne se lance que si toutes les précédentes sont `VALIDEE`. Une seule phase active à la fois. |
| Ordre des tâches | Champ **Ordre** + **Dépend de**. `scripts/backlog.py suivante` donne toujours la prochaine tâche exécutable. Une seule tâche `EN_COURS` à la fois. |
| Statuts de tâche | ⬜ `NON_FAIT` → 🔄 `EN_COURS` → 🟡 `A_VALIDER` → ✅ `VALIDE` (utilisateur) · ⛔ `BLOQUE` (hypothèse bloquante ouverte) |
| Statuts de phase | `NON_DEMARREE` → `EN_COURS` (utilisateur) → `A_VALIDER` (automatique) → `VALIDEE` (utilisateur) |
| Modèle | **opus** : complexe (architecture, sécurité, algorithmes, générateur) · **sonnet** : moyen (CRUD, pages, intégrations) · **haiku** : simple (configuration, données, documentation). Tâche → sous-agent `executeur-<modèle>`. « Modèle conseillé » de phase → modèle de la session d'orchestration (`/model`). |
| Test de validation | Exécuté par l'exécuteur, **rejoué** par le `verificateur`. Sans test réussi, pas de `A_VALIDER`. |
| Précision manquante | Fiche `backlog/HYPOTHESES.md` via `scripts/backlog.py hypothese-ajouter`. |
| Tables hors MLD initial | Toute nouvelle table : migration EF + mise à jour de `../Documentation/04-mld.md` dans la même tâche. |

## Tableau de bord

<!-- RESUME:DEBUT -->

_Mis à jour le 2026-10-16 12:54 par scripts/backlog.py — ne pas modifier à la main._

| Phase | Titre | Statut | Modèle | Avancement |
|---|---|---|---|---|
| P0 | Initialisation et outillage | ✅ VALIDEE | sonnet | 6/6 |
| P1 | Socle backend .NET 10 | 🔄 EN_COURS | opus | 0/11 |
| P2 | Socle frontend Blazor WebAssembly et BFF | ⬜ NON_DEMARREE | sonnet | 0/9 |
| P3 | Conteneurisation et intégration continue GitHub Actions | ⬜ NON_DEMARREE | sonnet | 0/5 |
| P4 | Module Organisation et périmètre hiérarchique | ⬜ NON_DEMARREE | sonnet | 0/8 |
| P5 | Projets et affectation en cascade | ⬜ NON_DEMARREE | opus | 0/10 |
| P6 | Planification et ordonnancement | ⬜ NON_DEMARREE | opus | 0/12 |
| P7 | Coût en temps | ⬜ NON_DEMARREE | sonnet | 0/8 |
| P8 | Reporting par niveau hiérarchique | ⬜ NON_DEMARREE | sonnet | 0/8 |
| P9 | Collaboration et services transverses | ⬜ NON_DEMARREE | sonnet | 0/5 |
| P10 | Modélisation et diagrammes | ⬜ NON_DEMARREE | opus | 0/9 |
| P11 | Générateur de code | ⬜ NON_DEMARREE | opus | 0/7 |
| P12 | Intégration GitLab | ⬜ NON_DEMARREE | sonnet | 0/6 |
| P13 | Déploiement Docker | ⬜ NON_DEMARREE | sonnet | 0/5 |
| P14 | Cadrage, exigences et conception des écrans | ⬜ NON_DEMARREE | sonnet | 0/6 |
| P15 | Qualité, recette et DevSecOps | ⬜ NON_DEMARREE | sonnet | 0/6 |
| P16 | Documentation, méthodes agiles, automatisation et API | ⬜ NON_DEMARREE | sonnet | 0/6 |
| P17 | Expérience utilisateur transverse et accessibilité | ⬜ NON_DEMARREE | sonnet | 0/8 |
| P18 | Sécurité Keycloak et durcissement | ⬜ NON_DEMARREE | opus | 0/7 |
| P19 | Performance, recette finale et mise en production | ⬜ NON_DEMARREE | sonnet | 0/6 |

<!-- RESUME:FIN -->

---

## Phase P0 · Initialisation et outillage
> **Statut phase** : VALIDEE · **Modèle conseillé** : sonnet

**Objectif** : dépôt Git propre, conventions, environnement vérifié, base de test opérationnelle. **Livrable** : un dépôt prêt à recevoir la solution .NET.

### P0-T01 · Initialiser le dépôt Git et la branche de phase
> **Statut** : VALIDE · **Modèle** : haiku · **Ordre** : 1 · **Dépend de** : —

**Explication** : `git init -b main` à la racine du projet, premier commit contenant l'existant (`CLAUDE.md`, `.claude/`, `backlog/`, `scripts/`, `database/`), puis branche `phase/P0-initialisation`. Le `.gitignore` existe déjà (secrets locaux dont `.claude/settings.local.json`, `.pgtest/`, .NET) : le vérifier **avant** le premier `git add`, le compléter si besoin. Créer `.gitattributes` (`* text=auto eol=lf`, `*.sql` et `*.md` en LF, `*.ps1` en CRLF).
**Test de validation** : `git log --oneline` montre au moins 2 commits ; `git status --porcelain` est vide ; `git check-ignore .pgtest/x bin/x .claude/settings.local.json` renvoie les trois chemins ; `git ls-files | grep -c settings.local` renvoie 0.

### P0-T02 · Script de vérification de l'environnement
> **Statut** : VALIDE · **Modèle** : haiku · **Ordre** : 2 · **Dépend de** : P0-T01

**Explication** : `scripts/verifier-env.py` affiche un tableau OK / MANQUANT / OPTIONNEL pour : .NET SDK 10, Python ≥ 3.12 avec le module openpyxl (export Excel du backlog), binaires PostgreSQL 18 (`PG_BIN`), Git, Node.js (optionnel : ponytail, motion), Docker (requis à partir de P3), uv et graphify (optionnels). Code de sortie 1 si un outil requis manque.
**Test de validation** : `python scripts/verifier-env.py` affiche le tableau et sort en 0 sur ce poste (dotnet, python, PostgreSQL, git présents) ; en masquant `PG_BIN` vers un dossier vide, il sort en 1.

### P0-T03 · Valider l'instance PostgreSQL de test et le schéma de référence
> **Statut** : VALIDE · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P0-T01

**Explication** : utiliser `scripts/pg-test.py` pour démarrer l'instance jetable, appliquer `database/01` et `02`, exécuter `database/03-tests-integrite.sql`, puis arrêter. Ajouter `scripts/tester-schema.py` qui automatise cela et vérifie que les 13 blocs « attendu ERREUR » échouent et que les autres réussissent (analyse de la sortie psql).
**Test de validation** : `python scripts/tester-schema.py` sort en 0 et affiche « 18/18 vérifications conformes » ; `.pgtest/` n'existe plus après exécution.

### P0-T04 · Conventions de build .NET centralisées
> **Statut** : VALIDE · **Modèle** : haiku · **Ordre** : 4 · **Dépend de** : P0-T01

**Explication** : `global.json` (SDK 10.0.4xx, `rollForward: latestFeature`), `Directory.Build.props` (`net10.0`, `Nullable enable`, `ImplicitUsings enable`, `TreatWarningsAsErrors true`, `AnalysisLevel latest-recommended`, `InvariantGlobalization false`), `Directory.Packages.props` (gestion centralisée des versions : EF Core 10, Npgsql 10, Mapperly, FluentValidation, Serilog, xUnit, FluentAssertions, bUnit, NetArchTest, Microsoft.AspNetCore.Components.WebAssembly et .Server), `.editorconfig` (style C# : file-scoped namespaces, `var` si évident, accolades, nommage `_champ`).
**Test de validation** : `dotnet --version` (dans le dossier) renvoie une version 10.0.4xx ; `dotnet new console -o .tmpcheck && dotnet build .tmpcheck` réussit avec les props appliquées (vérifier `TreatWarningsAsErrors` dans `obj/*.nuget.g.props` ou via `dotnet msbuild -getProperty:TreatWarningsAsErrors`) ; supprimer `.tmpcheck`.

### P0-T05 · Modèles GitLab et guide de contribution
> **Statut** : VALIDE · **Modèle** : haiku · **Ordre** : 5 · **Dépend de** : P0-T01

**Explication** : `.gitlab/merge_request_templates/Phase.md` (résumé de phase, tâches, preuves, hypothèses, checklist de revue), `.gitlab/issue_templates/Anomalie.md` et `Demande.md`, `CONTRIBUTING.md` (branches `phase/Pn-…`, commits `Pn-Tnn: …`, revue, règles des couches, comment lancer les tests).
**Test de validation** : les 4 fichiers existent ; `CONTRIBUTING.md` contient les sections « Branches », « Commits », « Architecture », « Tests » (`grep -c "^## "` ≥ 4).

### P0-T06 · Rapport d'outillage Claude Code
> **Statut** : VALIDE · **Modèle** : haiku · **Ordre** : 6 · **Dépend de** : P0-T02

**Explication** : `docs/OUTILLAGE.md` : état réel des plugins / skills (graphify, ponytail, ui-ux-pro-max, motion-design) détecté sur le poste (`~/.claude/skills`, `.claude/skills`, `~/.claude/plugins`), des hooks et des sous-agents du projet, avec la commande d'installation de chaque élément manquant (voir `scripts/installer-outils.ps1`). Si un outil manque, créer une hypothèse **non bloquante**.
**Test de validation** : `docs/OUTILLAGE.md` liste les 4 outils avec un statut chacun ; `python scripts/backlog.py contexte` s'exécute sans erreur.

---

## Phase P1 · Socle backend .NET 10
> **Statut phase** : EN_COURS · **Modèle conseillé** : opus

**Objectif** : solution en couches compilable, persistance PostgreSQL identique au schéma de référence, API démarrable. **Livrable** : `Gprojet.slnx` avec le projet unique `Gprojet-Backend` + API `/health` + tests d'architecture verts.

### P1-T01 · Créer la solution et le projet Gprojet-Backend
> **Statut** : EN_COURS · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : `Gprojet.slnx` ; **un seul projet backend** `src/Gprojet-Backend/Gprojet-Backend.csproj` (`Microsoft.NET.Sdk.Web`, `RootNamespace Gprojet.Backend`) avec les dossiers-couches `Web/`, `Orchestration/`, `Services/`, `Mapping/`, `Repositories/`, `Entities/`, `Contracts/`, `Infrastructure/`, `Common/` (espaces de noms alignés sur les dossiers) ; projet de tests `tests/Gprojet-Backend.Tests` (xUnit) avec `Unitaires/`, `Integration/`, `Architecture/`. Aucun autre projet (le front arrive en P2-T01). **Demandes utilisateur à intégrer dans cette tâche** : `Directory.Packages.props` → FluentAssertions en dernière **7.x** stable (licence Apache 2.0, H-017) au lieu de 8.x ; migrer `.gitlab/` vers `.github/` (`pull_request_template.md`, `ISSUE_TEMPLATE/Anomalie.md` et `Demande.md`) et adapter `CONTRIBUTING.md` (pull requests GitHub).
**Test de validation** : `dotnet build Gprojet.slnx` réussit sans avertissement ; `dotnet sln Gprojet.slnx list` affiche exactement 2 projets (`Gprojet-Backend`, `Gprojet-Backend.Tests`) ; `dotnet test` passe (test de fumée). `Directory.Packages.props` référence FluentAssertions 7.x ; `.gitlab/` n'existe plus et `.github/pull_request_template.md` existe.

### P1-T02 · Tests d'architecture par espace de noms
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 2 · **Dépend de** : P1-T01

**Explication** : les couches étant des dossiers d'un même projet, **ces tests remplacent le compilateur** comme garde. NetArchTest dans `tests/Gprojet-Backend.Tests/Architecture/`, une règle par interdiction entre espaces de noms `Gprojet.Backend.*` : Web ↛ Services / Repositories / Entities ; Orchestration ↛ implémentations de Repositories ; Services ↛ Contracts, Mapping, EF Core ; Entities ↛ toute autre couche sauf Common ; Contracts ↛ tout sauf BCL et FluentValidation (compilé dans le navigateur) ; types `*Dto` uniquement dans Contracts ; contrôleurs uniquement dans Web. Test « garde-fou » : des types de violation volontaire, définis dans le projet de test sous des espaces de noms simulés, sont bien détectés.
**Test de validation** : `dotnet test tests/Gprojet-Backend.Tests --filter Category=Architecture` → tous verts, ≥ 10 règles ; le garde-fou prouve qu'une violation fait échouer la règle (assert inversé).

### P1-T03 · Common : primitives transverses
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P1-T01

**Explication** : `Result<T>` / `Result`, exceptions métier (`RegleMetierException`, `AccesRefuseException`, `IntrouvableException`, `ConflitException`), `PageRequete` / `PageResultat<T>` (tri, filtre, taille max 200), `ICurrentUser` (Id agent, matricule, entité, rôles), `IHorloge` (testable), `IGenerateurCode` (codes lisibles).
**Test de validation** : `dotnet test tests/Gprojet-Backend.Tests --filter Category=Common` → tests de `Result`, pagination (bornes, taille max) et horloge factice verts.

### P1-T04 · Entités du domaine (6 schémas)
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 4 · **Dépend de** : P1-T03

**Explication** : `EntiteAuditable` (Id Guid v7, audit, suppression logique) et une classe par table de `database/01-schema.sql`, rangées par module (`Organisation`, `Projets`, `Planification`, `Temps`, `Commun`, `Modelisation`), avec propriétés de navigation, collections, et énumérations C# pour chaque `CHECK … IN`. `Historique` n'hérite pas d'`EntiteAuditable` (bigint, ajout seul).
**Test de validation** : `dotnet build` OK ; test de réflexion : nombre de types entités = 60 et chaque énumération a les mêmes valeurs que la contrainte SQL correspondante (test paramétré lisant `01-schema.sql`).

### P1-T05 · DbContext et configurations EF Core
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 5 · **Dépend de** : P1-T04

**Explication** : `GpDbContext` (Npgsql, extension `ltree`), une `IEntityTypeConfiguration<T>` par entité : schéma, noms `snake_case`, types (`numeric(p,s)`, `ltree`, `jsonb`, `text[]`, `inet`), `uuidv7()` par défaut, contraintes `CHECK`, index uniques **partiels** (`HasFilter`), index GiST, colonne générée `criticite`, `xmin` (concurrence), filtre global `!EstSupprime`, conversions enum → texte.
**Test de validation** : `dotnet build` OK ; `dotnet ef dbcontext info` réussit ; test unitaire : le modèle EF déclare 60 tables réparties dans les 6 schémas.

### P1-T06 · Migration initiale identique au schéma de référence
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 6 · **Dépend de** : P1-T05

**Explication** : migration `InitialCreate` (+ trigger d'ajout seul sur `commun.historique` et données de `02-donnees-reference.sql` via `migrationBuilder.Sql`). Écrire `scripts/comparer-schema.py` : crée deux bases sur l'instance de test (l'une par `01-schema.sql`, l'autre par `dotnet ef database update`), puis compare colonnes (type, nullabilité, défaut), contraintes, index (définition) et triggers via `pg_catalog`. Appliquer ensuite la migration à la **base de développement `gprojet`** (vide au départ ; connexion fournie par la variable `ConnectionStrings__Gp`, définie dans `.claude/settings.local.json` — ne jamais l'afficher ni la recopier).
**Test de validation** : `python scripts/comparer-schema.py` → « 0 écart » ; toute différence est listée et fait échouer le script ; `dotnet ef database update` sur `gprojet` réussit et `gprojet` contient 60 tables dans les 6 schémas.

### P1-T07 · Intercepteurs : audit, suppression logique, historique chaîné
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 7 · **Dépend de** : P1-T06

**Explication** : `SaveChangesInterceptor` qui renseigne `cree_*` / `modifie_*` depuis `ICurrentUser` et `IHorloge`, transforme un `Remove` en suppression logique, et écrit une ligne `commun.historique` par champ modifié avec l'empreinte `sha256(empreinte_precedente + contenu)` et un `correlation_id` par requête.
**Test de validation** : tests d'intégration sur `pg-test` : création → audit rempli ; suppression → `est_supprime = true` et ligne invisible via le filtre global ; modification → lignes d'historique avec chaîne d'empreintes valide (recalcul) ; altérer une ligne en SQL direct est détecté par le vérificateur de chaîne.

### P1-T08 · Unit of Work et repositories de base
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 8 · **Dépend de** : P1-T06

**Explication** : `IUnitOfWork` (CommitAsync, transaction explicite), classe de base **interne** `RepositoryBase<T>` (GetById, Ajouter, Supprimer logique, pagination), interfaces de repository par agrégat dans `Repositories/` (exposées via des interfaces consommées par les Services). Gestion de `DbUpdateConcurrencyException` → `ConflitException`.
**Test de validation** : tests d'intégration : commit atomique (échec au milieu → rien n'est écrit) ; deux mises à jour concurrentes de la même ligne → `ConflitException`.

### P1-T09 · Socle API : DI, erreurs, OpenAPI, logs, santé
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 9 · **Dépend de** : P1-T08

**Explication** : `Program.cs` minimal ; méthodes d'extension d'enregistrement par couche (`AddGpRepositories`, `AddGpServices`, `AddGpOrchestration`…) ; middleware d'exceptions → `ProblemDetails` (400/403/404/409/500 sans fuite de détail en production) ; OpenAPI natif + Scalar en Development ; Serilog (console JSON) ; `/health` (live) et `/health/ready` (base) ; chaîne de connexion via configuration (`ConnectionStrings:Gp`), fournie en local par la variable d'environnement `ConnectionStrings__Gp` (base `gprojet`, définie dans `.claude/settings.local.json`) ; **les tests n'utilisent jamais `gprojet`** : instance jetable `pg-test` ou `GP_TEST_CONNECTION`, qui écrasent explicitement cette valeur.
**Test de validation** : test d'intégration `WebApplicationFactory` : `/health` 200, `/health/ready` 200 sur `pg-test`, `/openapi/v1.json` 200, une `IntrouvableException` levée par un contrôleur de test → 404 au format `application/problem+json` ; `dotnet run` de l'API puis `curl -s localhost:<port>/health/ready` → 200 sur `gprojet`.

### P1-T10 · Authentification de développement et utilisateur courant
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 10 · **Dépend de** : P1-T09

**Explication** : `DevAuthHandler` (schéma « Dev », actif **uniquement** en environnement Development/Test) qui lit l'en-tête `X-Dev-Matricule`, charge l'agent et ses rôles déduits (responsable d'entité, délégation active) ; implémentation `ICurrentUser`. Keycloak le remplacera en P18.
**Test de validation** : tests d'intégration : sans en-tête → 401 ; matricule inconnu → 401 ; matricule valide → 200 et `ICurrentUser` renseigné ; en environnement Production le schéma « Dev » n'est pas enregistré.

### P1-T11 · Données de démonstration
> **Statut** : NON_FAIT · **Modèle** : haiku · **Ordre** : 11 · **Dépend de** : P1-T06

**Explication** : `database/04-donnees-demo.sql` : 1 direction, 3 divisions, 6 services, 25 agents fictifs (responsables nommés, fonctions, compétences), 2 délégations, quelques absences, jours fériés de l'année. Uniquement des noms fictifs. Option `--demo` dans `pg-test.py demarrer`.
**Test de validation** : `python scripts/pg-test.py demarrer --demo` puis requêtes de comptage : 10 entités, 25 agents, 10 responsables ; chaque service a un parent division (requête `ltree`).

---

## Phase P2 · Socle frontend Blazor WebAssembly et BFF
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : projet unique `Gprojet-Front` servi par `Gprojet-Backend` (BFF intégré), charte et layout définitifs, couches front en place. **Livrable** : coquille navigable authentifiée (auth de dev).

### P2-T01 · Projet Gprojet-Front, hébergement par le backend et tests bUnit
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : **un seul projet front** `src/Gprojet-Front/Gprojet-Front.csproj` (`Microsoft.NET.Sdk.BlazorWebAssembly`, `RootNamespace Gprojet.Front`) ; les DTO y sont des **fichiers liés** (`<Compile Include="..\Gprojet-Backend\Contracts\**\*.cs" Link="Contracts\%(RecursiveDir)%(Filename)%(Extension)" />`), jamais copiés ; `Gprojet-Backend` référence `Gprojet-Front` et le sert (`UseBlazorFrameworkFiles`, `MapFallbackToFile("index.html")`, paquet `Microsoft.AspNetCore.Components.WebAssembly.Server`) : même origine, pas de proxy ni de CORS ; tests `tests/Gprojet-Front.Tests` (bUnit). Arborescence des couches front : `Api/`, `Services/`, `Models/`, `Pages/`, `Components/`, `Layouts/`, `State/`, `Auth/`, `Resources/`, `wwwroot/css/`, `wwwroot/fonts/`, `wwwroot/lib/`, `wwwroot/images/`.
**Test de validation** : `dotnet build Gprojet.slnx` OK et `dotnet sln list` affiche exactement 4 projets ; test d'intégration backend : `GET /` → 200 HTML contenant `blazor.webassembly`, `GET /api/health` → 200 ; un DTO de `Gprojet-Backend/Contracts` est utilisé dans le front (compilation) ; règle d'architecture : le front ne référence aucun autre code du backend.

### P2-T02 · Charte HCP : intégration de app.css et des ressources
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P2-T01

**Explication** : appliquer `design/charte-hcp/README.md`. Copier `design/charte-hcp/app.css` dans `wwwroot/css/app.css` **sans autre modification** que le remplacement de l'`@import` Google Fonts par un `@font-face` local (Geist, licence OFL, fichiers dans `wwwroot/fonts/`). Créer `wwwroot/css/app.gp.css` (chargé après) : variantes de texte accessibles (`--success-text`, `--warning-text`, `--danger-text`, `--info-text`, `--purple-text`), badges des statuts GestionProjet (`.status-badge.a-faire`, `.a-valider`, `.bloque`, `.valide`…), classes manquantes, le tout à partir des variables de la charte. Auto-héberger Font Awesome Free 6 et Chart.js dans `wwwroot/lib/` ; logo HCP dans `wwwroot/images/`. `docs/DESIGN.md` : renvoi vers la charte et inventaire des classes utilisées / ajoutées.
**Test de validation** : un script compare `design/charte-hcp/app.css` et `wwwroot/css/app.css` : seule la ligne `@import` diffère ; `grep -rE "https?://" src/Gprojet-Front/wwwroot/index.html src/Gprojet-Front/wwwroot/css` ne renvoie rien (aucune ressource externe) ; aucune couleur hexadécimale dans `Pages/`, `Components/`, `Layouts/` ni dans `app.gp.css` hors des 5 variables de texte documentées ; contraste de chaque variable de texte ≥ 4,5:1 (calcul dans un test).

### P2-T03 · Layout principal et navigation par rôle
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P2-T02

**Explication** : `MainLayout` identique à celui de la GED HCP : `.page` > `aside.sidebar` (`.sidebar-brand` avec logo HCP, sections `.nav-section-label`, `.nav-item` avec icône et `.nav-badge`, pied `.user-row`) + `.main-area` > `.topbar` (bouton de repli, `.breadcrumb-nav`, `.search-box` avec raccourci Ctrl+K, notifications) + `.content-area`. Menu construit selon les rôles / politiques (`AuthorizeView`) ; repli du menu et mode mobile (classes `sidebar-collapsed`, `mobile-open` dans `app.gp.css`).
**Test de validation** : tests bUnit : le rendu contient `.page .sidebar`, `.main-area .topbar` et `.content-area` ; un agent ne voit pas l'entrée « Portefeuille direction », un directeur la voit ; l'action de repli ajoute `sidebar-collapsed`.

### P2-T04 · Mouvement selon la charte (motion-design)
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P2-T02

**Explication** : réutiliser **uniquement** les mouvements de la charte (`0.15s ease` pour les survols, `fadeIn 0.25s` / `.animate-in` pour les apparitions, `0.6s` pour les barres de progression) ; **motion-design** sert à décider où les appliquer (apparition des pages et des listes, dialogues, repli du menu), pas à créer d'autres durées. Neutralisation complète sous `prefers-reduced-motion: reduce` dans `app.gp.css`. Documenter dans `docs/DESIGN.md`.
**Test de validation** : `app.gp.css` contient un bloc `@media (prefers-reduced-motion: reduce)` qui neutralise `transition` et `animation` ; aucune durée autre que 0.1s, 0.15s, 0.25s, 0.6s dans les CSS du projet (grep) ; aucune bibliothèque JS d'animation ajoutée (`git diff --stat` sur `wwwroot/lib` vide).

### P2-T05 · Couche Api : clients HTTP typés
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : P2-T01

**Explication** : `ApiClientBase` (sérialisation System.Text.Json, conversion `ProblemDetails` → `ApiException` typée avec erreurs de validation, annulation), enregistrement des clients via `IHttpClientFactory` en adresses relatives (même origine que le backend), premier client `SanteApiClient`.
**Test de validation** : tests unitaires avec `HttpMessageHandler` simulé : 200 → DTO ; 400 `problem+json` → `ApiException` contenant les erreurs par champ ; 404 → `ApiException.Statut == 404`.

### P2-T06 · Authentification côté front (BFF, mode dev)
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : P2-T05

**Explication** : endpoints BFF dans `Gprojet-Backend/Web/Auth/` : `/bff/login` (dev : choix d'un matricule de démo), `/bff/logout`, `/bff/user` ; cookie `HttpOnly`, `Secure`, `SameSite=Strict` ; jeton anti-CSRF exigé sur les appels mutants ; `BffAuthenticationStateProvider` côté WASM ; page de connexion de développement.
**Test de validation** : tests d'intégration BFF : `/bff/user` sans cookie → 401 ; après login dev → 200 avec matricule ; POST `/api/...` sans en-tête anti-CSRF → 400/403 ; test bUnit de `AuthorizeView` avec le provider.

### P2-T07 · État partagé et services front de base
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 7 · **Dépend de** : P2-T06

**Explication** : `State/UtilisateurCourantState`, `State/PerimetreState` (entité, niveau, chemin), `Services/NotificationUiService` (snackbar), `Services/ChargementService` (indicateur global), gestion centralisée des `ApiException` (message utilisateur, journal console).
**Test de validation** : tests bUnit : un composant abonné à `PerimetreState` se rafraîchit au changement ; une `ApiException` 403 affiche le message « Accès refusé » via le snackbar.

### P2-T08 · Internationalisation fr / en / ar (RTL)
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 8 · **Dépend de** : P2-T03

**Explication** : `Resources/*.resx` (fr par défaut, en, ar), sélecteur de langue persistant (cookie BFF), `dir="rtl"` sur `<html>` et règles RTL dans `app.gp.css` (menu latéral à droite) pour l'arabe, formats de date et de nombre selon la culture.
**Test de validation** : tests bUnit : en culture `ar-MA` le layout est rendu avec `dir="rtl"` ; un libellé du menu change selon la culture ; aucun texte en dur dans `Layouts/` (contrôle : test qui échoue si une chaîne littérale française apparaît dans un `.razor` du layout).

### P2-T09 · Pages système
> **Statut** : NON_FAIT · **Modèle** : haiku · **Ordre** : 9 · **Dépend de** : P2-T08

**Explication** : pages Accueil (placeholder du tableau de bord), 404, Accès refusé, Erreur inattendue (avec identifiant de corrélation), Chargement initial.
**Test de validation** : tests bUnit : chaque page se rend sans exception et contient son titre traduit.

---

## Phase P3 · Conteneurisation et intégration continue GitHub Actions
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : images Docker, environnement local complet par docker compose, intégration continue **GitHub Actions** sur le dépôt `hajar-hamid-coder/gprojet`. **Livrable** : `docker compose up` fonctionnel et `.github/workflows/ci.yml`. ⚠️ Nécessite Docker (hypothèse H-001).

### P3-T01 · Dockerfile de l'application
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : `deploy/docker/Dockerfile` unique, multi-étapes, qui publie `Gprojet-Backend` (le front publié est servi par le backend) (`sdk:10.0` → `aspnet:10.0` chiseled ou alpine), utilisateur non-root, `HEALTHCHECK`, variables `ASPNETCORE_*`, pas de secret dans l'image ; `.dockerignore`.
**Test de validation** : `docker build -f deploy/docker/Dockerfile -t gprojet:test .` réussit ; `docker run --rm gprojet:test id -u` ≠ 0 ; l'image contient le front (`wwwroot/_framework`) ; taille d'image affichée dans la preuve.

### P3-T02 · docker compose de développement
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P3-T01

**Explication** : `deploy/docker-compose.yml` : `postgres:18` (volume, base vide), service `migrations` (bundle EF `dotnet ef migrations bundle` qui crée le schéma — pas les scripts SQL, pour rester aligné sur les migrations), `app` (image unique), profil `keycloak` (préparé pour P18), réseau dédié, dépendances sur la santé ; `deploy/.env.example` sans valeur sensible.
**Test de validation** : `docker compose -f deploy/docker-compose.yml up -d --wait` puis `curl -s http://localhost:8080/api/health` → 200 et `curl -s http://localhost:8080/` → HTML ; `docker compose down -v` propre.

### P3-T03 · Tests en CI avec PostgreSQL de service
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : —

**Explication** : rendre les tests d'intégration indépendants de `pg-test.py` via la variable `GP_TEST_CONNECTION` (si définie : base fournie par la CI ; sinon : instance locale jetable). Fixture xUnit commune.
**Test de validation** : `dotnet test` local (instance jetable) vert ; avec `GP_TEST_CONNECTION` pointant sur l'instance démarrée manuellement, les mêmes tests passent.

### P3-T04 · Workflow GitHub Actions
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P3-T01, P3-T03

**Explication** : `.github/workflows/ci.yml` déclenché sur push et pull request : jobs `build`, `test` (service `postgres:18`, `GP_TEST_CONNECTION`, rapport de tests et couverture publiés comme artefacts), `qualite` (`dotnet format --verify-no-changes`), `securite` (CodeQL C#, `dependency-review` sur les pull requests, scan d'image Trivy), `paquet` (build et push de l'image vers **GitHub Container Registry** `ghcr.io` sur `main` et les tags, avec `GITHUB_TOKEN`), `deploiement` (`workflow_dispatch` + environnement protégé, préparé pour P13). Cache NuGet, versions d'actions épinglées. Vérifier que les modèles `.github/` (faits en P1-T01) sont cohérents avec le workflow.
**Test de validation** : `python scripts/verifier-ci.py` (à écrire, parse YAML) confirme les 6 jobs, leurs dépendances, le service PostgreSQL et l'absence de secret en clair ; les modèles `.github/` sont présents. L'exécution réelle sur GitHub se constate après le push de l'utilisateur (signalé dans le bilan de phase).

### P3-T05 · Documentation de déploiement local
> **Statut** : NON_FAIT · **Modèle** : haiku · **Ordre** : 5 · **Dépend de** : P3-T02, P3-T04

**Explication** : `docs/DEPLOIEMENT.md` : prérequis, variables, démarrage compose, migrations, dépannage, description du workflow GitHub Actions, des secrets à créer (Settings → Secrets and variables → Actions) et de la publication d'image sur ghcr.io.
**Test de validation** : le document contient les sections Prérequis, Démarrage, Variables, Pipeline, Dépannage ; chaque commande citée existe dans le dépôt.

---

## Phase P4 · Module Organisation et périmètre hiérarchique
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : organigramme, agents, délégations, absences, et le **service de périmètre** sur lequel repose toute la sécurité des données. **Livrable** : module Organisation complet (API + pages).

### P4-T01 · Service de périmètre et rôles déduits
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 1 · **Dépend de** : —

**Explication** : `PerimetreService` : rôle hiérarchique déduit (directeur / chef de division / chef de service / agent) depuis l'entité dirigée et les délégations actives à la date du jour ; chemin `ltree` du périmètre ; `IPerimetreCourant` injectable ; extension `IQueryable<Projet>.DansPerimetre()` et équivalents Phase / Tâche / Ticket / Saisie (membre du projet inclus) ; politiques `EstChef`, `EstDirecteur`…
**Test de validation** : tests unitaires et d'intégration (données de démo) : matrice de 12 cas (4 rôles × visibilité au-dessus / à côté / en dessous) ; délégation hors dates → aucun droit ; agent membre d'un projet d'un autre service → voit ce projet uniquement.

### P4-T02 · Organigramme : API
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P4-T01

**Explication** : cas d'usage complets (DTO, validateurs, orchestrateur, service, repository, contrôleur) : créer / modifier / désactiver une entité, déplacer une entité (recalcul du chemin de tout le sous-arbre en une requête), nommer un responsable. Règles : service → parent division, division → parent direction ; un agent ne dirige qu'une entité.
**Test de validation** : tests d'intégration : déplacement d'une division → chemins de ses services et projets portés mis à jour ; service rattaché à une direction → 400 ; double responsabilité → 409.

### P4-T03 · Agents, rattachements et compétences : API
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P4-T02

**Explication** : CRUD agents ; **mutation** (clôture du rattachement, ouverture du nouveau, mise à jour de `entite_id`) ; fonction ; compétences avec niveau ; recherche paginée (nom, matricule, entité, compétence).
**Test de validation** : tests : mutation → 2 rattachements dont 1 ouvert ; tentative de second rattachement ouvert → refus ; recherche par compétence renvoie les bons agents.

### P4-T04 · Délégations, absences et calendrier : API
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P4-T01

**Explication** : délégations (période, droits, entité ; seul le responsable de l'entité délègue), absences (types, demi-journée), jours fériés ; service `CalendrierService.JoursOuvres(agent, période)` utilisé plus tard par le CPM et le plan de charge.
**Test de validation** : tests : délégation active → le délégataire obtient `PeutCreerProjet` ; `JoursOuvres` exclut week-ends, fériés et absences (cas avec demi-journée = 0,5).

### P4-T05 · Pages organigramme
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : P4-T02

**Explication** : arbre interactif (composant maison `ArbreOrganisation` au style de la charte) avec responsable et effectif, fiche entité (membres, projets portés), formulaires création / déplacement / nomination ; écrans selon la charte HCP (`design/charte-hcp/README.md`).
**Test de validation** : tests bUnit : l'arbre affiche 3 niveaux à partir de données simulées ; le formulaire refuse un parent incohérent (message de validation partagé) ; les actions sont masquées pour un agent.

### P4-T06 · Pages agents
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : P4-T03

**Explication** : liste filtrable et paginée, fiche agent (rattachements, compétences, absences, projets), dialogue de mutation, édition des compétences.
**Test de validation** : tests bUnit : filtre par entité ; dialogue de mutation → appel du service avec les bons paramètres ; affichage de l'historique des rattachements.

### P4-T07 · Pages délégations, absences et calendrier
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 7 · **Dépend de** : P4-T04

**Explication** : gestion des délégations par le chef (avec bandeau « vous agissez par délégation de … »), saisie des absences, calendrier des jours fériés (admin).
**Test de validation** : tests bUnit : le bandeau de délégation apparaît quand l'utilisateur agit par délégation ; validation des dates (fin ≥ début).

### P4-T08 · Documentation fonctionnelle du module
> **Statut** : NON_FAIT · **Modèle** : haiku · **Ordre** : 8 · **Dépend de** : P4-T05, P4-T06, P4-T07

**Explication** : `docs/modules/organisation.md` : règles métier, rôles et droits, endpoints (lien OpenAPI), écrans.
**Test de validation** : le document liste chaque endpoint du module (comparaison automatique avec `/openapi/v1.json` via un petit script ou test).

---

## Phase P5 · Projets et affectation en cascade
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : opus

**Objectif** : création réservée aux chefs, cascade Directeur → Division → Service → Personnel, portefeuille filtré. **Livrable** : module Projets complet et étanche entre périmètres.

### P5-T01 · Référentiels : types de projet et modèles de phases
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : API + pages d'administration des types de projet et de leurs modèles de phases (ordre, critères, mode d'enchaînement).
**Test de validation** : tests : réordonnancement des modèles de phases sans trou ni doublon ; suppression d'un type utilisé → 409.

### P5-T02 · Création de projet
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 2 · **Dépend de** : P5-T01

**Explication** : réservée aux chefs et délégataires ; code `PRJ-AAAA-NNNN` sans collision en concurrence (séquence annuelle) ; entité créatrice, chemin initial ; instanciation des phases depuis le modèle du type ; statut `BROUILLON` → `EN_AFFECTATION` (direction / division) ou `EN_COURS` (service).
**Test de validation** : tests : un agent → 403 ; un chef de service → projet `EN_COURS` porté par son service ; 20 créations parallèles → 20 codes distincts ; les phases du modèle DEV sont créées (7).

### P5-T03 · Affectation en cascade
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 3 · **Dépend de** : P5-T02

**Explication** : affecter (cible = enfant direct de la source), accepter (met à jour entité porteuse + chemin), renvoyer (motif obligatoire, revient à l'émetteur), réaffecter, annuler ; chaîne `precedente_id` ; une seule affectation active ; événements de domaine pour notifications (consommés en P9) ; historique.
**Test de validation** : tests couvrant chaque transition et chaque refus (cible non enfant, double affectation, renvoi sans motif, acteur hors périmètre) ; scénario complet directeur → division → service → acceptation : chemin final = chemin du service.

### P5-T04 · Équipe projet
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P5-T03

**Explication** : le chef du service porteur ajoute / retire des membres (agents de son service, prestataires et observateurs autorisés), quotas, désignation du chef de projet.
**Test de validation** : tests : agent d'un autre service → refus ; somme des quotas d'un agent sur ses projets > 100 % → avertissement renvoyé (pas de blocage) ; chef de projet obligatoirement membre.

### P5-T05 · Jalons et registre RAID
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : P5-T02

**Explication** : CRUD jalons (lien phase optionnel, retard calculé) et éléments RAID (criticité, responsable, échéance, statut).
**Test de validation** : tests : un risque sans probabilité → 400 ; jalon dépassé non atteint → statut `EN_RETARD` calculé.

### P5-T06 · Politiques d'autorisation et tests anti-fuite
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 6 · **Dépend de** : P5-T03, P5-T04

**Explication** : politiques `PeutCreerProjet`, `PeutAffecterDivision`, `PeutAffecterService`, `PeutGererEquipe`, `PeutVoirProjet` appliquées à tous les contrôleurs Projets ; un projet hors périmètre renvoie **404** (pas 403, pour ne pas révéler son existence).
**Test de validation** : suite de tests d'intégration paramétrée parcourant **tous** les endpoints Projets avec 4 profils : aucun accès hors périmètre (404), aucune liste ne contient de projet hors périmètre.

### P5-T07 · Pages portefeuille et création
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 7 · **Dépend de** : P5-T06

**Explication** : portefeuille (grille filtrable : statut, type, priorité, météo, entité), assistant de création en étapes, fiche projet à onglets (synthèse, phases, équipe, jalons, RAID, historique).
**Test de validation** : tests bUnit : le bouton « Nouveau projet » est absent pour un agent ; l'assistant valide chaque étape avant la suivante.

### P5-T08 · Pages d'affectation
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 8 · **Dépend de** : P5-T07

**Explication** : file « Projets à affecter » par chef, dialogues affecter / accepter / renvoyer, frise de la chaîne d'affectation.
**Test de validation** : tests bUnit : le renvoi exige un motif ; la frise affiche les étapes dans l'ordre chronologique.

### P5-T09 · Pages équipe, jalons, RAID
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 9 · **Dépend de** : P5-T07

**Explication** : onglets de la fiche projet : gestion de l'équipe, jalons, matrice des risques (probabilité × impact).
**Test de validation** : tests bUnit : la matrice place un risque P4×I5 dans la bonne cellule ; seuls les membres du service sont proposés dans l'ajout d'équipe.

### P5-T10 · Tâches planifiées et alerte de non-affectation
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 10 · **Dépend de** : P5-T03

**Explication** : infrastructure de jobs (Quartz.NET, persistance PostgreSQL) dans `Infrastructure/` ; job quotidien : projet en attente d'affectation au-delà de `alerte.delai_affectation_jours` → événement d'alerte au chef concerné.
**Test de validation** : test avec `IHorloge` simulée : projet affecté il y a 6 jours (seuil 5) → 1 alerte ; 4 jours → 0.

---

## Phase P6 · Planification et ordonnancement
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : opus

**Objectif** : phases, tâches, tickets, workflows, dépendances, chemin critique. **Livrable** : planification complète avec Gantt et Kanban.

### P6-T01 · Workflows configurables
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : administration des workflows (statuts, catégories, couleurs, statut initial, transitions et rôles autorisés) ; service `WorkflowService.PeutTransiter(element, vers, utilisateur)`.
**Test de validation** : tests : transition non déclarée → refus ; transition réservée `CHEF_PROJET` refusée à un membre ; un workflow sans statut `TERMINE` est invalide.

### P6-T02 · Phases : gestion et ordre
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P6-T01

**Explication** : CRUD phases, réordonnancement transactionnel sans trou, mode d'enchaînement, critères, livrables.
**Test de validation** : tests : déplacer la phase 4 en position 2 → ordres 1..n continus ; mode `CHEVAUCHEMENT` sans pourcentage → 400.

### P6-T03 · Validation de fin de phase (gate)
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 3 · **Dépend de** : P6-T02

**Explication** : soumission d'une phase, circuit de validation multi-niveaux (chef de service puis chef de division selon paramétrage), rejet motivé → nouveau tour, règle de démarrage de N+1 selon le mode d'enchaînement.
**Test de validation** : tests : N+1 ne démarre pas tant que N n'est pas validée (mode séquentiel) ; démarre à 60 % si chevauchement à 50 % ; rejet → tour 2 sans perdre le tour 1.

### P6-T04 · Tâches : gestion, WBS, estimation
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P6-T02

**Explication** : CRUD tâches, code WBS recalculé, ordre, mode d'exécution, responsable (membre du projet), estimation à trois points (PERT : (O + 4M + P) / 6), récurrence.
**Test de validation** : tests : WBS « 2.3 » après déplacement ; estimation O=4, M=6, P=14 → 7 h ; responsable non membre → 400.

### P6-T05 · Dépendances, cycles et contrôle de démarrage
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 5 · **Dépend de** : P6-T04

**Explication** : dépendances FD / DD / FF / DF avec délai ; détection de cycle (parcours en profondeur sur le graphe du projet) ; démarrage d'une tâche : mode BLOQUANT → refus si prédécesseur non terminé ; mode SOUPLE → dérogation justifiée obligatoire.
**Test de validation** : tests : A→B→C puis C→A → refus « cycle » ; démarrer B avant A en mode bloquant → refus ; en mode souple sans justification → 400, avec → OK et dérogation enregistrée.

### P6-T06 · Moteur de chemin critique (CPM)
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 6 · **Dépend de** : P6-T05

**Explication** : passes avant et arrière sur jours ouvrés (calendrier, fériés, absences du responsable, capacité), 4 types de liens et délais ; dates prévues, marges totale et libre, `est_critique` ; recalcul en cascade lors d'un glissement + alerte au chef de projet.
**Test de validation** : tests sur 3 réseaux de référence avec résultats calculés à la main (dont un manuel de gestion de projet) : dates, marges et chemin critique exacts ; glissement de 3 jours d'une tâche critique → date de fin projet +3 jours ouvrés.

### P6-T07 · Tickets
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 7 · **Dépend de** : P6-T01, P6-T04

**Explication** : CRUD tickets, numérotation atomique (`prochain_numero_ticket`), types et workflows, rang, liens, tickets bloquants empêchant la clôture de la tâche.
**Test de validation** : tests : 50 créations parallèles → numéros 1..50 sans trou ni doublon ; clôturer une tâche avec un ticket bloquant ouvert → 409.

### P6-T08 · Remontée d'avancement et de charge
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 8 · **Dépend de** : P6-T07

**Explication** : recalcul des caches ticket → tâche → phase → projet (charge estimée, reste à faire, avancement) à chaque modification, dans la même transaction.
**Test de validation** : tests : passer un ticket en catégorie TERMINE met à jour l'avancement de la tâche, de la phase et du projet avec les valeurs attendues.

### P6-T09 · Baselines
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 9 · **Dépend de** : P6-T06

**Explication** : figer le planning (photo des dates et charges), comparer planifié / baseline / réel, écarts par tâche et par phase.
**Test de validation** : tests : après glissement, la comparaison montre l'écart exact par tâche ; deux baselines coexistent.

### P6-T10 · Pages liste, Kanban et fiches
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 10 · **Dépend de** : P6-T08

**Explication** : vue liste hiérarchique (phase > tâche > ticket), Kanban des tickets par statut avec glisser-déposer respectant les transitions, fiches tâche et ticket, « Ma prochaine tâche » et « Mes tickets ».
**Test de validation** : tests bUnit : un dépôt vers une colonne non autorisée est refusé visuellement et non envoyé ; « Ma prochaine tâche » affiche la tâche prête la plus prioritaire.

### P6-T11 · Gantt et réseau PERT
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 11 · **Dépend de** : P6-T06, P6-T10

**Explication** : composant Gantt (bibliothèque JS sous licence libre via JS Interop, choix consigné en hypothèse) : barres, dépendances, jalons, chemin critique en surbrillance, déplacement → recalcul ; vue PERT (graphe) ; accessibilité clavier minimale et alternative tabulaire.
**Test de validation** : test bUnit de l'enveloppe (données transmises au module JS) ; test d'intégration : le déplacement envoie la bonne requête et le recalcul revient ; vérification manuelle décrite dans la preuve (capture).

### P6-T12 · Documentation du module
> **Statut** : NON_FAIT · **Modèle** : haiku · **Ordre** : 12 · **Dépend de** : P6-T11

**Explication** : `docs/modules/planification.md` : règles d'ordonnancement, modes, CPM (formules), workflows par défaut.
**Test de validation** : le document contient les sections Ordonnancement, CPM, Workflows, Écrans et un exemple chiffré.

---

## Phase P7 · Coût en temps
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : saisie, validation hiérarchique, prévu / consommé / reste à faire, plan de charge. **Livrable** : module Temps complet.

### P7-T01 · Référentiels et paramètres du temps
> **Statut** : NON_FAIT · **Modèle** : haiku · **Ordre** : 1 · **Dépend de** : —

**Explication** : administration des catégories d'activité (projet / hors projet) et des paramètres (heures par jour, seuils).
**Test de validation** : tests : désactiver une catégorie utilisée la masque à la saisie sans casser l'historique.

### P7-T02 · Feuilles de temps et saisies
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P7-T01

**Explication** : feuille hebdomadaire (créée à la demande), saisies avec règles : catégorie projet ⇒ tâche obligatoire, ticket appartenant à la tâche, total ≤ 24 h par jour, agent membre du projet, lecture seule si feuille validée ou verrouillée.
**Test de validation** : tests pour chaque règle (cas nominal + refus) ; saisie hors projet acceptée sans tâche.

### P7-T03 · Chronomètre
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P7-T02

**Explication** : démarrer / arrêter un chronomètre depuis un ticket (un seul actif par agent), conversion en saisie arrondie au quart d'heure.
**Test de validation** : tests : 1 h 08 → 1,25 h ; second chronomètre → arrêt automatique du premier.

### P7-T04 · Circuit de validation des temps
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 4 · **Dépend de** : P7-T02

**Explication** : soumettre, valider / rejeter (motif) par le chef du service de rattachement **à la date de la semaine** (historique des rattachements, délégations), verrouillage de période, consolidation par le chef de division.
**Test de validation** : tests : agent muté en cours de mois → la semaine antérieure est validée par l'ancien chef ; rejet sans motif → 400 ; période verrouillée → modification refusée.

### P7-T05 · Prévu, consommé, reste à faire et alertes
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : P7-T04

**Explication** : agrégation des temps validés en jours-homme par tâche / phase / projet, écarts (jours, %), reprévision de la date de fin selon la vélocité réelle, job d'alerte au-delà du seuil de charge consommée.
**Test de validation** : tests chiffrés : estimé 10 j, consommé 8,5 j → 85 % → alerte (seuil 80) ; reprévision conforme à la formule documentée.

### P7-T06 · Plan de charge et taux d'occupation
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : P7-T05

**Explication** : charge affectée vs capacité par agent, service, division, direction (jours ouvrés, absences, quotas) ; taux d'occupation incluant le hors-projet ; surcharge / sous-charge.
**Test de validation** : tests chiffrés sur les données de démo : taux d'occupation d'un service = valeur calculée à la main dans le test.

### P7-T07 · Pages saisie, chronomètre et validation
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 7 · **Dépend de** : P7-T04

**Explication** : grille hebdomadaire (lignes tâches / tickets, colonnes jours, totaux), widget chronomètre dans la barre d'application, file de validation des chefs.
**Test de validation** : tests bUnit : total journalier > 24 h signalé avant envoi ; la file de validation n'affiche que les agents du périmètre.

### P7-T08 · Page plan de charge
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 8 · **Dépend de** : P7-T06

**Explication** : carte de chaleur agents × semaines (couleurs de surcharge du thème), filtres par entité et compétence.
**Test de validation** : tests bUnit : une cellule à 120 % porte la classe « surcharge » ; filtre par compétence.

---

## Phase P8 · Reporting par niveau hiérarchique
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : tableaux de bord Directeur / Division / Service / Agent avec drill-down et exports. **Livrable** : reporting complet, rapide et étanche.

### P8-T01 · Requêtes de reporting
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 1 · **Dépend de** : —

**Explication** : requêtes de lecture dédiées (projections ou SQL) **filtrées par périmètre** : synthèse portefeuille (statut, type, priorité, météo), comparatifs divisions / services, charges et écarts, retards, affectations en attente et délais d'affectation, validations en attente, jalons à 30/60/90 jours, risques et vulnérabilités critiques.
**Test de validation** : tests chiffrés sur un jeu de données dédié (valeurs attendues écrites dans le test) ; un chef de division ne reçoit aucune ligne d'une autre division ; chaque requête < 300 ms sur ce jeu (mesure dans le test).

### P8-T02 · Tableau de bord Directeur
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P8-T01

**Explication** : indicateurs de synthèse, comparatif des divisions (graphiques), projets non affectés, retards, validations en attente, jalons, risques ; cartes `.stat-card`, graphiques Chart.js aux couleurs de la charte, ergonomie via ui-ux-pro-max.
**Test de validation** : tests bUnit : chaque widget gère les états vide / chargement / erreur ; les valeurs affichées correspondent au DTO fourni.

### P8-T03 · Tableau de bord Chef de division
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P8-T01

**Explication** : mêmes indicateurs sur la division, comparatif des services, projets à affecter, charge par service.
**Test de validation** : tests bUnit analogues à P8-T02.

### P8-T04 · Tableau de bord Chef de service
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P8-T01

**Explication** : projets du service, équipes à constituer, charge par agent, feuilles à valider, tâches bloquées et critiques, tickets en retard.
**Test de validation** : tests bUnit analogues à P8-T02.

### P8-T05 · Tableau de bord Agent
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : P8-T01

**Explication** : ma prochaine tâche, mes tickets par priorité et ordre, temps de la semaine, échéances.
**Test de validation** : tests bUnit analogues à P8-T02.

### P8-T06 · Navigation de drill-down
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : P8-T02, P8-T03, P8-T04

**Explication** : composant fil d'Ariane Direction → Division → Service → Projet → Phase → Tâche → Ticket → Agent, conservation des filtres dans l'URL, retour arrière cohérent.
**Test de validation** : tests bUnit : cliquer sur une division filtre le tableau de bord et ajoute un niveau au fil d'Ariane ; l'URL contient les filtres.

### P8-T07 · Exports Excel et PDF
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 7 · **Dépend de** : P8-T01

**Explication** : export Excel (bibliothèque libre) et PDF au format officiel (en-tête, logo de l'entité) des rapports standard ; choix des bibliothèques et licences consigné en hypothèse.
**Test de validation** : tests : le fichier Excel contient les colonnes et le nombre de lignes attendus ; le PDF est lisible (extraction du texte contient le titre du rapport).

### P8-T08 · Rapports planifiés par e-mail
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 8 · **Dépend de** : P8-T07

**Explication** : job hebdomadaire / mensuel envoyant à chaque chef le rapport de son périmètre ; envoi SMTP via `Infrastructure/` (en développement : dépôt dans un dossier de fichiers `.eml`).
**Test de validation** : test : exécution du job en développement → un `.eml` par chef, contenant uniquement les projets de son périmètre.

---

## Phase P9 · Collaboration et services transverses
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : commentaires, pièces jointes, notifications temps réel, historique consultable. **Livrable** : collaboration sur tout élément.

### P9-T01 · Commentaires et mentions
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : commentaires polymorphes (projet, phase, tâche, ticket, diagramme…), fils de discussion, `@mention` d'un agent du projet, édition marquée, droits selon le périmètre de l'élément commenté.
**Test de validation** : tests : commenter un ticket hors périmètre → 404 ; une mention crée un événement de notification ; un fil de 3 niveaux est restitué dans l'ordre.

### P9-T02 · Pièces jointes
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : —

**Explication** : téléversement en flux (taille max et types autorisés paramétrés), stockage disque configurable hors racine web, empreinte SHA-256, téléchargement contrôlé par le périmètre ; point d'extension pour un antivirus (hypothèse).
**Test de validation** : tests : fichier > taille max → 413 ; extension interdite → 400 ; l'empreinte stockée correspond au fichier ; téléchargement hors périmètre → 404.

### P9-T03 · Notifications temps réel
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P9-T01

**Explication** : consommation des événements de domaine (affectation, mention, validation, échéance…) → notifications en base, diffusion SignalR, centre de notifications côté front, préférences par type, résumé quotidien par e-mail.
**Test de validation** : test d'intégration SignalR : une affectation provoque la réception du message par le client du chef ciblé et pas par un autre ; test bUnit du compteur non lu.

### P9-T04 · Abonnements (observateurs)
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P9-T03

**Explication** : nouvelle table `commun.abonnement` (migration + mise à jour du MLD), s'abonner / se désabonner d'un élément, notifications aux abonnés.
**Test de validation** : `python scripts/comparer-schema.py` toujours à 0 écart après mise à jour de `database/01-schema.sql` ; un abonné reçoit la notification d'un changement de statut.

### P9-T05 · Historique et vérification d'intégrité
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : —

**Explication** : frise d'historique par élément (qui, quoi, quand, avant / après), endpoint d'administration qui vérifie la chaîne d'empreintes et signale la première rupture.
**Test de validation** : tests : la frise d'un ticket modifié 3 fois contient 3 entrées ; une altération SQL directe est détectée à la bonne ligne.

---

## Phase P10 · Modélisation et diagrammes
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : opus

**Objectif** : métamodèle éditable, cohérence, transformation MCD → MLD → SQL, éditeurs de diagrammes. **Livrable** : module Modélisation, capable de reproduire le MLD de la plateforme.

### P10-T01 · API du métamodèle
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : CRUD modèles (versions), modules, entités, attributs, énumérations et valeurs, associations et pattes, index, méthodes et paramètres ; validations (noms techniques `snake_case`, rôle obligatoire sur une association réflexive, propriétaire unique d'un attribut).
**Test de validation** : tests d'intégration couvrant chaque ressource et chaque règle de validation.

### P10-T02 · Contrôle de cohérence d'un modèle
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 2 · **Dépend de** : P10-T01

**Explication** : rapport de cohérence : entité sans identifiant, association à moins de 2 pattes, cardinalités manquantes, héritage cyclique, noms en conflit (table / colonne générée), attribut libellé absent, énumération vide.
**Test de validation** : un modèle de test contenant volontairement 7 défauts → rapport listant exactement ces 7 défauts ; le modèle de la plateforme → 0 défaut.

### P10-T03 · Transformation MCD → MLD (règles R1 à R10)
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 3 · **Dépend de** : P10-T02

**Explication** : générer `table_bd`, `colonne`, `cle_etrangere` à partir du modèle selon R1…R10 (`../Documentation/03-mcd.md` §8), avec traçabilité vers la source et conservation des surcharges manuelles (`est_surchargee`) signalées en cas de conflit.
**Test de validation** : un test par règle (10 tests minimum) avec MLD attendu explicite ; régénération après surcharge → surcharge conservée et conflit signalé.

### P10-T04 · Génération SQL (MPD) PostgreSQL
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 4 · **Dépend de** : P10-T03

**Explication** : produire le script DDL (schémas, tables, colonnes typées via `correspondance_type`, PK, FK, CHECK d'énumérations, index uniques partiels, colonnes techniques selon options) dans l'ordre des dépendances.
**Test de validation** : le SQL généré pour un modèle de test s'exécute sans erreur sur l'instance `pg-test` ; idempotence textuelle (deux générations identiques).

### P10-T05 · Import / export et rétro-ingénierie
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 5 · **Dépend de** : P10-T04

**Explication** : export / import d'un modèle complet en JSON versionné ; rétro-ingénierie d'une base PostgreSQL existante (`pg_catalog`) → MLD → MCD (tables d'association reconnues comme associations N:N).
**Test de validation** : aller-retour export → import → export identique ; rétro-ingénierie de la base de référence → 60 tables et associations N:N reconnues (`entite_interface`, etc.).

### P10-T06 · Dogfooding : la plateforme se décrit elle-même
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 6 · **Dépend de** : P10-T05

**Explication** : construire le modèle de la plateforme (par rétro-ingénierie puis ajustements : rôles, natures d'association, options), générer son SQL et le comparer à `database/01-schema.sql` avec `scripts/comparer-schema.py`. Consigner chaque écart irréductible.
**Test de validation** : `python scripts/comparer-schema.py --gauche database/01-schema.sql --droite <sql généré>` → 0 écart, ou écarts listés **et** justifiés dans `docs/modules/modelisation.md`.

### P10-T07 · Éditeur visuel MCD / classes
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 7 · **Dépend de** : P10-T01

**Explication** : canevas de diagramme (bibliothèque JS libre via JS Interop, choix en hypothèse) synchronisé avec le modèle sémantique : création d'entités, attributs, associations et cardinalités, héritage ; positions stockées dans `element_diagramme` ; une même entité apparaît dans les vues MCD et classes.
**Test de validation** : test bUnit de l'enveloppe et test d'intégration : créer une association dans l'éditeur crée les 2 pattes en base ; déplacer un élément ne modifie que sa position.

### P10-T08 · Diagrammes documentaires
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 8 · **Dépend de** : P10-T01

**Explication** : séquence, cas d'utilisation, activité, états, composants, déploiement, C4 en texte (Mermaid / PlantUML) avec aperçu ; BPMN via éditeur dédié (licence à vérifier, hypothèse) ; liens vers exigences et tickets.
**Test de validation** : tests bUnit : l'aperçu se met à jour au changement de texte ; un diagramme invalide affiche l'erreur de syntaxe sans casser la page.

### P10-T09 · Versions, comparaison, validation et exports
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 9 · **Dépend de** : P10-T07, P10-T08

**Explication** : versionner modèles et diagrammes, comparer deux versions (ajouts / suppressions / modifications), circuit de validation (architecte, chef de projet), exports PNG / SVG / PDF.
**Test de validation** : tests : la comparaison de deux versions liste exactement les changements ; un modèle `FIGE` refuse toute modification.

---

## Phase P11 · Générateur de code
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : opus

**Objectif** : produire depuis le modèle un projet .NET/Blazor complet qui compile, conforme à l'architecture GP. **Livrable** : générateur Scriban + régénération incrémentale + dogfooding.

### P11-T01 · Moteur de génération Scriban
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 1 · **Dépend de** : —

**Explication** : modèle de rendu (modules, entités, attributs typés C# / SQL, navigations, DTO, options), fonctions d'aide (PascalCase, camelCase, snake_case, pluriel français), portées ENTITE / MODULE / MODELE, résolution de `chemin_sortie`, templates par défaut embarqués et surchargeables en base.
**Test de validation** : tests unitaires : rendu d'un template de test sur un modèle à 2 entités → sortie attendue exacte ; pluriels (« projet » → « projets », « travail » → « travaux » par table d'exceptions).

### P11-T02 · Templates backend
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 2 · **Dépend de** : P11-T01

**Explication** : templates Entité, configuration EF, DTO (lecture, création, modification, liste), validateurs, mapper Mapperly, repository, service, orchestrateur, contrôleur — en `*.g.cs` et classes `partial`, respectant les couches GP.
**Test de validation** : génération pour un modèle d'exemple (3 entités, 1 N:N, 1 énumération) puis `dotnet build` du résultat → 0 erreur, 0 avertissement ; les tests d'architecture GP appliqués au projet généré passent.

### P11-T03 · Templates socle : solution, base, Docker, CI
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 3 · **Dépend de** : P11-T02

**Explication** : même structure que GestionProjet — **deux projets** `<Nom>-Backend` (couches en dossiers) et `<Nom>-Front` (+ tests et tests d'architecture) : `.slnx`, `.csproj`, `Directory.Build.props`, `Program.cs`, DbContext, migration initiale ou script SQL, Dockerfiles, docker-compose, `.gitlab-ci.yml`, README.
**Test de validation** : le projet généré compile, sa migration s'applique sur `pg-test`, et son API répond 200 sur `/health` et sur un `GET` de liste.

### P11-T04 · Templates frontend
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P11-T03

**Explication** : clients Api, services front, modèles de formulaire, pages Liste / Détail / Formulaire au balisage de la charte HCP — `.explorer-table`, `.form-group`, `.toolbar-btn` — embarquée dans le socle généré (contrôles selon `controle_ui`, listes déroulantes pour les associations, validation partagée).
**Test de validation** : le front généré compile ; test bUnit généré pour une page Liste passe.

### P11-T05 · Régénération incrémentale et prévisualisation
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 5 · **Dépend de** : P11-T04

**Explication** : régénérer sans toucher au code manuel (fichiers non `.g.cs`), détecter les conflits (fichier généré modifié à la main), prévisualiser le diff fichier par fichier avant écriture, rapport d'exécution.
**Test de validation** : tests : ajout d'un attribut → seuls les `.g.cs` concernés changent ; un `.g.cs` modifié à la main est signalé et non écrasé sans confirmation.

### P11-T06 · Interface de génération et sortie ZIP
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : P11-T05

**Explication** : pages configuration (stack, architecture, dépôt cible), lancement, prévisualisation, téléchargement ZIP, historique des exécutions ; personnalisation des templates par l'équipe.
**Test de validation** : tests : le ZIP contient l'arborescence attendue ; l'historique enregistre version du modèle, nombre de fichiers et statut.

### P11-T07 · Dogfooding : générer le squelette de GestionProjet
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 7 · **Dépend de** : P11-T06

**Explication** : générer depuis le modèle de la plateforme (P10-T06) et comparer avec le code réel : structure à deux projets, entités, configurations EF, DTO. Rapport d'écarts et corrections des templates.
**Test de validation** : le squelette généré compile ; `docs/modules/generation.md` contient le rapport d'écarts avec, pour chaque écart, la décision (corrigé / assumé).

---

## Phase P12 · Intégration GitLab
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : relier projets, tickets et code GitLab. **Livrable** : traçabilité commit / MR / pipeline ↔ ticket. ⚠️ Instance GitLab requise (H-002).

### P12-T01 · Client GitLab
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : client API v4 dans `Infrastructure/` (jeton depuis la configuration sécurisée, pagination, gestion des limites de débit, erreurs) ; tests avec un serveur HTTP simulé.
**Test de validation** : tests unitaires (WireMock.Net) : pagination sur 3 pages, 429 → nouvelle tentative, 401 → erreur explicite.

### P12-T02 · Création et liaison des dépôts
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P12-T01

**Explication** : à la création d'un projet (option), création du groupe et des dépôts (backend, frontend, infra), branches protégées, modèles de MR ; liaison projet ↔ dépôts (nouvelle table + migration + MLD).
**Test de validation** : tests avec serveur simulé : séquence d'appels attendue ; `comparer-schema.py` à 0 écart après mise à jour du schéma de référence.

### P12-T03 · Envoi du code généré vers GitLab
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P12-T02

**Explication** : pousser une génération sur une branche dédiée via l'API commits (actions create / update), puis ouvrir une MR.
**Test de validation** : test avec serveur simulé : un seul commit contenant tous les fichiers, branche et MR créées.

### P12-T04 · Webhooks entrants
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P12-T01

**Explication** : réception push / MR / pipeline (jeton secret vérifié), rattachement par numéro de ticket dans branche, commit ou titre de MR ; transitions automatiques (MR ouverte → En revue, MR fusionnée → À tester) ; imputation « Time 2h ».
**Test de validation** : tests : charge utile réelle d'exemple → lien créé et statut changé ; jeton invalide → 401 ; « Time 2h » → saisie de 2 h source GITLAB.

### P12-T05 · Vues GitLab dans les tickets
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : P12-T04

**Explication** : onglet « Code » d'un ticket (branches, commits, MR, état du pipeline), bouton « Créer la branche » nommée `feature/<numéro>-<slug>`.
**Test de validation** : tests bUnit : affichage des états de pipeline ; nom de branche généré conforme.

### P12-T06 · Releases et notes de version
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : P12-T04

**Explication** : tags et releases liés aux phases, notes de version générées depuis les tickets fusionnés, correspondance milestones ↔ phases.
**Test de validation** : test : les notes de version listent exactement les tickets fusionnés entre deux tags.

---

## Phase P13 · Déploiement Docker
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : environnements, déploiements via pipeline, validation avant production, DORA. **Livrable** : module Déploiement.

### P13-T01 · Environnements : modèle et API
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : tables `environnement` et `deploiement` (modélisées dans le module Modélisation, migration, MLD mis à jour) ; CRUD environnements (dev, recette, préprod, prod : URL, serveurs, responsable).
**Test de validation** : `comparer-schema.py` à 0 écart ; tests CRUD.

### P13-T02 · Déclenchement des déploiements
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P13-T01

**Explication** : déclencher le job manuel du pipeline GitLab pour une version et un environnement, suivre son état, historique (qui, quand, version, tickets inclus).
**Test de validation** : test avec serveur GitLab simulé : déclenchement → déploiement `EN_COURS` puis `SUCCES` sur webhook pipeline.

### P13-T03 · État des environnements
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P13-T01

**Explication** : sonde de santé périodique des URL d'environnement, version déployée, lien vers les journaux.
**Test de validation** : test avec un serveur local : `/health` 200 → OK ; arrêt → KO après le délai.

### P13-T04 · Validation avant production et rollback
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P13-T02

**Explication** : déploiement en production soumis à approbation hiérarchique ; rollback = redéploiement du tag précédent.
**Test de validation** : tests : déploiement prod sans approbation → refusé ; rollback cible le tag précédent réussi.

### P13-T05 · Indicateurs DORA et pages
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : P13-T04

**Explication** : fréquence de déploiement, délai de livraison, taux d'échec, temps de rétablissement ; pages environnements et historique.
**Test de validation** : tests chiffrés sur un historique simulé ; tests bUnit des pages.

---

## Phase P14 · Cadrage, exigences et conception des écrans
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : modules Brainstorming, Analyse / exigences, Conception des écrans, traçabilité. **Livrable** : de l'idée à l'écran spécifié.

### P14-T01 · Modèle de données cadrage / exigences / écrans
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 1 · **Dépend de** : —

**Explication** : modéliser (dans le module Modélisation) puis migrer : idée, session d'atelier, post-it, vote, exigence, user story, critère d'acceptation, règle de gestion, glossaire, demande de changement, écran, champ d'écran, action d'écran, liens de traçabilité ; mettre à jour MLD et schéma de référence.
**Test de validation** : `comparer-schema.py` à 0 écart ; contrôle de cohérence du modèle (P10-T02) à 0 défaut.

### P14-T02 · Brainstorming et tableau blanc
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P14-T01

**Explication** : fiches idées, tableau blanc collaboratif temps réel (SignalR) : post-its, regroupements, votes, minuteur, mode anonyme ; conversion post-it → exigence / tâche.
**Test de validation** : test d'intégration SignalR à 2 clients : un post-it ajouté par l'un apparaît chez l'autre ; la conversion crée l'exigence liée.

### P14-T03 · Modèles d'ateliers et priorisation
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P14-T02

**Explication** : SWOT, 5 Why, Persona, Lean Canvas, MoSCoW, cartographie des parties prenantes ; scoring RICE, valeur / effort ; note de cadrage générée.
**Test de validation** : tests : score RICE calculé exact ; la note de cadrage contient les sections du modèle.

### P14-T04 · Exigences, user stories et changements
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P14-T01

**Explication** : exigences fonctionnelles / non fonctionnelles numérotées, user stories, critères Gherkin, règles de gestion RG-xxx, glossaire, demandes de changement avec analyse d'impact en jours-homme et validation.
**Test de validation** : tests : numérotation continue par projet ; une demande de changement validée crée une nouvelle version de l'exigence sans perdre l'ancienne.

### P14-T05 · Conception des écrans
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : P14-T04

**Explication** : inventaire des écrans, fiche écran (composants, champs liés aux attributs du modèle, actions, règles), arborescence et parcours, wireframes simples en glisser-déposer ; ces fiches alimentent les templates de pages du générateur.
**Test de validation** : tests : un champ lié à un attribut obligatoire hérite de la contrainte ; génération de page (P11) utilisant la fiche écran → ordre des champs respecté.

### P14-T06 · Matrice de traçabilité
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 6 · **Dépend de** : P14-T05

**Explication** : matrice exigence ↔ cas d'utilisation ↔ écran ↔ entité ↔ ticket ↔ test ↔ MR, détection des trous (exigence sans test, écran sans exigence…), export.
**Test de validation** : jeu de données avec 3 trous connus → la matrice signale exactement ces 3 trous.

---

## Phase P15 · Qualité, recette et DevSecOps
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : recette outillée et sécurité intégrée au cycle. **Livrable** : modules Qualité et Sécurité.

### P15-T01 · Modèle de données qualité et sécurité
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 1 · **Dépend de** : —

**Explication** : plan de test, cas de test, campagne, exécution, détail de vulnérabilité (CVSS, CWE, CVE, actif, SLA), checklist de sécurité, menace STRIDE ; migration, MLD.
**Test de validation** : `comparer-schema.py` à 0 écart ; contrôle de cohérence à 0 défaut.

### P15-T02 · Plans, cas, campagnes et exécutions
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P15-T01

**Explication** : cahier de recette généré depuis les critères d'acceptation, campagnes par version et environnement, exécution pas à pas, création d'une anomalie depuis un test en échec (liée à l'exigence).
**Test de validation** : tests : une exécution en échec crée un ticket ANOMALIE_RECETTE lié ; couverture des exigences calculée exacte.

### P15-T03 · Résultats automatisés et qualité de code
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P15-T02

**Explication** : import des rapports JUnit des pipelines GitLab ; intégration SonarQube (si disponible, hypothèse) : dette, couverture, problèmes.
**Test de validation** : test : un rapport JUnit d'exemple → exécutions créées avec les bons statuts.

### P15-T04 · Gestion des vulnérabilités
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : P15-T01

**Explication** : calcul CVSS 3.1, SLA de correction selon la criticité, import des rapports GitLab SAST / DAST / dépendances / conteneurs → tickets VULNERABILITE (dédoublonnés).
**Test de validation** : tests : vecteur CVSS de référence → score exact ; import du même rapport deux fois → aucun doublon.

### P15-T05 · Checklists de sécurité et menaces
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : P15-T01

**Explication** : checklists OWASP Top 10 / ASVS par phase, modélisation STRIDE liée aux diagrammes d'architecture, analyse de risques simplifiée.
**Test de validation** : tests : une checklist incomplète empêche la validation de la phase si le paramètre l'exige.

### P15-T06 · PV de recette
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : P15-T02

**Explication** : génération du PV (VABF / VSR) depuis une campagne, signature / validation métier tracée.
**Test de validation** : test : le PV liste les cas passés / échoués et les réserves ; la validation est enregistrée dans l'historique.

---

## Phase P16 · Documentation, méthodes agiles, automatisation et API
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : wiki et livrables générés, Scrum, automatisations, API publique, imports. **Livrable** : modules 11, 21, 23, 24.

### P16-T01 · Wiki de projet
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : pages Markdown par projet (arborescence, versions, comparaison, liens vers éléments), modèles de pages ; migration + MLD.
**Test de validation** : tests : restaurer une version antérieure ; un lien vers un ticket hors périmètre n'en révèle pas le titre.

### P16-T02 · Génération des livrables documentaires
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 2 · **Dépend de** : P16-T01

**Explication** : SFG / SFD, dossier de conception (diagrammes inclus), DAT, dictionnaire de données, cahier de recette, en Word et PDF au format officiel, depuis les données saisies.
**Test de validation** : tests : le dictionnaire de données contient toutes les entités du modèle ; le document Word s'ouvre (validation OpenXML) et contient les titres attendus.

### P16-T03 · Mode Scrum
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : —

**Explication** : sprints dans une phase (mode hybride), planification selon la capacité, planning poker, rétrospectives (actions → tickets), burndown, burnup, vélocité, CFD.
**Test de validation** : tests chiffrés : burndown d'un sprint simulé ; capacité du sprint = somme des disponibilités.

### P16-T04 · Moteur d'automatisation
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 4 · **Dépend de** : —

**Explication** : règles « Quand (événement) / Si (conditions) / Alors (actions) » : assignation, notification, changement de statut, escalade ; protection contre les boucles ; journal d'exécution.
**Test de validation** : tests : règle « ticket bloqué > 3 jours → notifier le chef » déclenchée au bon moment ; deux règles qui se déclenchent mutuellement → arrêt après la limite et alerte.

### P16-T05 · API publique et webhooks sortants
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : —

**Explication** : API versionnée (`/api/v1`), jetons d'accès personnels (portée, expiration, révocation), limitation de débit, webhooks sortants signés (HMAC).
**Test de validation** : tests : jeton révoqué → 401 ; dépassement du débit → 429 ; signature HMAC vérifiable par le destinataire.

### P16-T06 · Imports Jira, Redmine, Trello, CSV, Excel
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : P16-T05

**Explication** : assistants d'import avec correspondance des champs et des statuts, simulation avant import, rapport.
**Test de validation** : tests : un export Jira d'exemple → tâches et tickets créés avec les bons statuts ; la simulation n'écrit rien.

---

## Phase P17 · Expérience utilisateur transverse et accessibilité
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : recherche, productivité, PWA, accessibilité RGAA, tests E2E. **Livrable** : application fluide et accessible.

### P17-T01 · Recherche globale et palette de commandes
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : recherche plein texte PostgreSQL (configuration française, `tsvector` + index GIN) filtrée par périmètre ; palette Ctrl+K (navigation, actions) ; raccourcis clavier documentés.
**Test de validation** : tests : la recherche « affectation » trouve « affecter » (racinisation) ; aucun résultat hors périmètre ; test bUnit de la palette au clavier.

### P17-T02 · Vues enregistrées et filtres avancés
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : —

**Explication** : filtres combinables, vues personnelles et partagées, langage de requête simple (`statut = EN_COURS ET priorite >= HAUTE`) avec analyseur et messages d'erreur.
**Test de validation** : tests de l'analyseur (requêtes valides et invalides) ; une vue partagée respecte le périmètre de chaque lecteur.

### P17-T03 · Favoris, récents, « Ma journée »
> **Statut** : NON_FAIT · **Modèle** : haiku · **Ordre** : 3 · **Dépend de** : —

**Explication** : favoris, éléments récents, page « Ma journée » (tickets du jour, réunions, temps à saisir).
**Test de validation** : tests bUnit : un favori ajouté apparaît dans le menu ; « Ma journée » liste les échéances du jour.

### P17-T04 · PWA
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 4 · **Dépend de** : —

**Explication** : manifeste, service worker (cache des ressources statiques, lecture hors ligne des derniers éléments consultés), invitation d'installation.
**Test de validation** : le manifeste est valide (champs requis) ; test E2E : page consultée accessible hors ligne en lecture.

### P17-T05 · Audit et corrections d'accessibilité
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : —

**Explication** : audit automatisé axe-core via Playwright .NET sur les pages principales, corrections, déclaration d'accessibilité (RGAA / WCAG 2.2 AA).
**Test de validation** : test E2E : 0 violation axe « serious » ou « critical » sur les 15 pages principales.

### P17-T06 · Arabe RTL et revue i18n
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : —

**Explication** : revue complète des traductions, mise en miroir correcte des composants (Gantt, Kanban, fil d'Ariane), formats de date.
**Test de validation** : test qui échoue si une clé de ressource existe en fr mais pas en en / ar ; capture E2E en arabe sans débordement.

### P17-T07 · Accueil guidé et aide contextuelle
> **Statut** : NON_FAIT · **Modèle** : haiku · **Ordre** : 7 · **Dépend de** : —

**Explication** : parcours d'accueil par rôle, info-bulles d'aide, liens vers le manuel.
**Test de validation** : tests bUnit : le parcours diffère selon le rôle ; il ne s'affiche qu'une fois.

### P17-T08 · Tests E2E des parcours critiques
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 8 · **Dépend de** : P17-T05

**Explication** : Playwright .NET : création projet → affectation en cascade → équipe → phases / tâches / tickets → saisie et validation des temps → tableau de bord directeur.
**Test de validation** : `dotnet test tests/Gprojet-E2E` vert sur l'environnement docker compose.

---

## Phase P18 · Sécurité Keycloak et durcissement
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : opus

**Objectif** : authentification réelle (Keycloak, LDAP / AD) et durcissement ASVS. **Livrable** : application prête pour l'homologation.

### P18-T01 · Realm Keycloak
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 1 · **Dépend de** : —

**Explication** : export JSON versionné du realm (client confidentiel du backend pour le BFF, client de l'API publique, rôles, mappers, politique de sessions), service Keycloak dans docker compose, fédération LDAP / AD paramétrée (hypothèse sur l'annuaire).
**Test de validation** : `docker compose --profile keycloak up` importe le realm ; obtention d'un jeton de test pour un utilisateur de démo.

### P18-T02 · BFF OIDC et API JWT
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 2 · **Dépend de** : P18-T01

**Explication** : BFF : code + PKCE, cookie sécurisé, rafraîchissement, déconnexion globale ; API : validation JWT (audience, émetteur, durée) ; correspondance `keycloak_sub` ↔ agent (premier login par matricule).
**Test de validation** : tests d'intégration avec Keycloak de test : connexion complète, jeton expiré → rafraîchi, déconnexion → cookie invalidé ; jeton d'une autre audience → 401.

### P18-T03 · MFA et politiques de session
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : P18-T01

**Explication** : MFA (TOTP) obligatoire pour les rôles de chef et d'administrateur, politique de mots de passe, durée de session, verrouillage.
**Test de validation** : test : un chef sans MFA configuré est redirigé vers l'enrôlement.

### P18-T04 · Revue de sécurité ASVS niveau 2
> **Statut** : NON_FAIT · **Modèle** : opus · **Ordre** : 4 · **Dépend de** : P18-T02

**Explication** : en-têtes (CSP stricte, HSTS, X-Content-Type-Options, Referrer-Policy), anti-CSRF, limitation de débit, validation des téléversements, injection SQL / XSS, journalisation de sécurité ; corrections ; rapport `docs/SECURITE.md`.
**Test de validation** : tests automatisés des en-têtes et du CSRF ; scan DAST de base (ZAP baseline via conteneur) sans alerte haute ; rapport complété.

### P18-T05 · Sauvegardes et restauration
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : —

**Explication** : sauvegarde planifiée (`pg_dump` chiffré, rétention), procédure de restauration documentée et testée.
**Test de validation** : script de test : sauvegarde → base détruite → restauration → comptages identiques.

### P18-T06 · Confidentialité et données personnelles
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : —

**Explication** : restrictions par classification (DIFFUSION_RESTREINTE : membres nommés uniquement), export des données d'un agent, anonymisation d'un agent parti, durées de conservation.
**Test de validation** : tests : un directeur ne voit pas le détail d'un projet DIFFUSION_RESTREINTE dont il n'est pas membre (seulement sa ligne agrégée) ; l'anonymisation conserve les temps mais retire l'identité.

### P18-T07 · Retrait de l'authentification de développement en production
> **Statut** : NON_FAIT · **Modèle** : haiku · **Ordre** : 7 · **Dépend de** : P18-T02

**Explication** : garantir que `DevAuthHandler` et la page de connexion de développement n'existent qu'en Development / Test.
**Test de validation** : test d'intégration en environnement Production : l'en-tête `X-Dev-Matricule` est ignoré (401) et `/bff/login` redirige vers Keycloak.

---

## Phase P19 · Performance, recette finale et mise en production
> **Statut phase** : NON_DEMARREE · **Modèle conseillé** : sonnet

**Objectif** : tenue en charge, observabilité, documentation d'exploitation, recette et go-live. **Livrable** : version 1.0 en production.

### P19-T01 · Jeu de données volumineux et tests de charge
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 1 · **Dépend de** : —

**Explication** : générateur de données (200 projets, 5 000 tâches, 100 000 tickets, 2 ans de temps), scénarios de charge (NBomber) sur tableaux de bord, listes et saisie des temps.
**Test de validation** : rapport NBomber : p95 < 2 s sur les tableaux de bord et < 500 ms sur les listes paginées avec 50 utilisateurs simultanés.

### P19-T02 · Optimisations
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 2 · **Dépend de** : P19-T01

**Explication** : analyse des plans d'exécution (`EXPLAIN ANALYZE`), index manquants, requêtes N+1, cache de lecture ; mise à jour du schéma de référence si index ajoutés.
**Test de validation** : nouveau passage NBomber conforme aux seuils ; `comparer-schema.py` à 0 écart.

### P19-T03 · Observabilité
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 3 · **Dépend de** : —

**Explication** : OpenTelemetry (traces, métriques, journaux), profil docker compose d'observabilité, tableaux de bord et alertes (erreurs 5xx, latence, file de jobs).
**Test de validation** : une requête produit une trace corrélée navigateur → backend → base visible dans l'outil ; une alerte se déclenche sur erreurs simulées.

### P19-T04 · Documentation d'exploitation et manuels
> **Statut** : NON_FAIT · **Modèle** : haiku · **Ordre** : 4 · **Dépend de** : —

**Explication** : `docs/EXPLOITATION.md` (installation, configuration, sauvegarde, mise à jour, supervision) et manuels utilisateur par rôle (directeur, chef de division, chef de service, agent, administrateur).
**Test de validation** : chaque manuel couvre les écrans de son rôle (liste de contrôle comparée au menu par rôle) ; chaque commande citée existe.

### P19-T05 · Recette finale
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 5 · **Dépend de** : P19-T02, P19-T03

**Explication** : campagne de recette complète dans le module Qualité sur l'environnement de recette, correction des anomalies bloquantes, PV de recette.
**Test de validation** : PV généré : 0 anomalie bloquante ou majeure ouverte ; tests E2E verts.

### P19-T06 · Mise en production
> **Statut** : NON_FAIT · **Modèle** : sonnet · **Ordre** : 6 · **Dépend de** : P19-T05

**Explication** : tag de version 1.0, images publiées, déploiement de production via le module Déploiement avec approbation, liste de contrôle de mise en production, plan de retour arrière.
**Test de validation** : liste de contrôle cochée dans `docs/MISE-EN-PRODUCTION.md` ; `/health/ready` de production à 200 ; procédure de rollback répétée en préproduction.
