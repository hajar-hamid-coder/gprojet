# GestionProjet — plateforme de gestion des projets IT de l'État

Plateforme qui couvre tout le cycle d'un projet IT : brainstorming → exigences → modélisation (UML, MERISE) → **génération de code** → pilotage Projet → Phase → Tâche → Ticket → temps (jours-homme) → GitLab → Docker → recette.
Organigramme : Directeur → Chef de division → Chef de service → agents. Seuls les chefs créent des projets ; affectation en cascade ; chacun ne voit que son périmètre et ce qui est en dessous.
Hors périmètre : facturation, budget, coûts financiers, IA dans le produit.

## Documents de référence (lire à la demande, pas en entier)

| Sujet | Fichier |
|---|---|
| 25 modules fonctionnels | `../Documentation/01-fonctionnalites.md` |
| Architecture, couches, stack | `../Documentation/02-architecture.md` |
| MCD + métamodèle du générateur | `../Documentation/03-mcd.md` |
| MLD (60 tables, règles d'intégrité) | `../Documentation/04-mld.md` |
| Schéma SQL de référence | `database/01-schema.sql`, `02-donnees-reference.sql`, `03-tests-integrite.sql` |
| **Charte graphique (obligatoire)** | `design/charte-hcp/README.md` + `design/charte-hcp/app.css` (copie de la GED HCP) |
| Base de développement | `gprojet` sur `localhost:5432` (vide au départ), connexion par la variable `ConnectionStrings__Gp` définie dans `.claude/settings.local.json` (non versionné) |
| Pilotage | `backlog/BACKLOG.md` (source), `backlog/HYPOTHESES.md`, `backlog/JOURNAL.md` ; `backlog/BACKLOG.xlsx` = vue Excel régénérée automatiquement (ne jamais l'éditer) |

## Mode de travail autonome (OBLIGATOIRE)

Tu es l'**orchestrateur**. L'utilisateur lance une phase, tu exécutes **toutes** ses tâches sans t'arrêter, puis tu lui demandes de valider la phase.

- Statuts de tâche : `NON_FAIT` → `EN_COURS` → `A_VALIDER` → (`VALIDE` posé par l'utilisateur) ; `BLOQUE` si une hypothèse bloquante attend une réponse.
- Statuts de phase : `NON_DEMARREE` → `EN_COURS` → `A_VALIDER` (automatique quand toutes les tâches sont A_VALIDER) → `VALIDEE` (utilisateur).
- **Tu ne modifies jamais `BACKLOG.md` / `HYPOTHESES.md` à la main** : uniquement `python scripts/backlog.py` (`suivante`, `voir`, `tache`, `note`, `hypothese-ajouter`, `etat`, `contexte`).
- **Réservé à l'utilisateur** : `/lancer-phase`, `/valider-phase`, `/refuser-tache`, `/repondre-hypothese`. Un hook bloque ces commandes si tu les tentes.
- Pour chaque tâche, suis le skill **`executer-tache`** (ordre, délégation au bon modèle, vérification, commit, preuve).
- Un hook `Stop` te relance tant qu'il reste une tâche exécutable : ne demande pas « je continue ? », continue.
- Quand la phase passe `A_VALIDER` : écris le bilan de phase dans ta réponse (tâches, preuves, hypothèses ouvertes, points à vérifier) et demande `/valider-phase Pn`. Ne lance jamais la phase suivante.

### Reprise après interruption (OBLIGATOIRE)

À chaque démarrage de session (nouvelle session, reprise, `/clear`, contexte compacté), le hook `SessionStart` injecte l'état du backlog, l'état Git et l'environnement. **Avant toute autre action** :
- tâche `EN_COURS` signalée, fichiers non commités, ou utilisateur qui tape `/reprendre` → applique le skill **`reprendre`** : constater (backlog, Git, notes du journal), remettre l'environnement d'aplomb, puis **terminer la tâche interrompue sans repartir de zéro** et sans jamais jeter de modifications (au pire `git stash`) ;
- phase `A_VALIDER` → re-présenter le bilan et attendre la validation ;
- aucune phase active → attendre `/lancer-phase`.
Pendant l'exécution, trace la progression avec `python scripts/backlog.py note <ID> "…"` (délégation, verdict, commit) : c'est ce qui permet à la session suivante de reprendre exactement où tu t'es arrêté.

### Choix du modèle

Chaque tâche indique `Modèle : opus | sonnet | haiku`. Délègue-la au sous-agent du même nom :
`executeur-opus` (complexe : architecture, sécurité du périmètre, CPM, générateur, MCD→MLD), `executeur-sonnet` (moyen : CRUD, pages, services courants), `executeur-haiku` (simple : configuration, données, documentation). Puis fais vérifier par `verificateur`.
Le « Modèle conseillé » d'une phase est celui que l'utilisateur sélectionne (`/model`) pour la session d'orchestration.

### Hypothèses

S'il manque une précision : `python scripts/backlog.py hypothese-ajouter --tache Pn-Tnn --bloquante oui|non --question "…" --hypothese "…" --impact "…"`.
- Non bloquante : continue avec l'hypothèse par défaut, et signale-la dans le bilan de phase.
- Bloquante : `tache Pn-Tnn BLOQUE`, puis passe à la tâche exécutable suivante.

## Architecture (non négociable)

**Deux projets applicatifs, pas un de plus** : `src/Gprojet-Backend` (ASP.NET Core : API + BFF, sert aussi le front) et `src/Gprojet-Front` (Blazor WebAssembly) ; tests dans `tests/Gprojet-Backend.Tests` et `tests/Gprojet-Front.Tests`.

.NET 10 · Blazor WebAssembly + BFF intégré au backend · ASP.NET Core Web API (contrôleurs) · EF Core 10 + Npgsql · PostgreSQL 18 · Keycloak (intégré en dernier) · Docker · GitLab.

Couches = **dossiers / espaces de noms** de `Gprojet-Backend` : `Web → Orchestration → Services → Repositories → Entities`, plus `Mapping`, `Contracts` (DTO partagés avec le front par fichiers liés), `Infrastructure`, `Common`. Le compilateur ne protège plus ces frontières : les tests NetArchTest (`tests/Gprojet-Backend.Tests/Architecture`) sont la garde, ne jamais les affaiblir.
- **Les DTO ne descendent jamais sous l'Orchestration ; les entités ne remontent jamais au-dessus.**
- Contrôleurs → orchestrateurs uniquement. Orchestration = cas d'usage + transaction (`IUnitOfWork`) + mapping. Services = règles métier sur entités. Repositories par agrégat, pas de `IRepository<T>` générique exposé.
- Mapping : **Mapperly**. Validation : **FluentValidation** dans `Contracts/` (BCL + FluentValidation uniquement, car compilé aussi dans le front). Erreurs : `ProblemDetails`.
- Toute requête sur les projets passe par le **périmètre** (`PerimetreService`, chemin `ltree`). Un oubli = fuite de données = bloquant.
- Reporting : requêtes de lecture dédiées (projections), hors flux Repository → Mapping.
- Front : `Api/` (clients typés) → `Services/` → `Models/` → `Pages/`, plus `Components/`, `Layouts/`, `State/`, `Auth/`, `Resources/`. Les pages n'appellent jamais `HttpClient` directement. UI : **charte HCP** (`design/charte-hcp/`) avec composants Razor maison, Font Awesome Free et Chart.js auto-hébergés ; **pas de MudBlazor**, aucune ressource chargée depuis Internet.
- Modules métier = dossiers dans chaque couche (`Organisation`, `Projets`, `Planification`, `Temps`, `Reporting`, `Commun`, `Modelisation`, `Generation`, `Gitlab`, `Deploiement`…).
- Code généré par le générateur : fichiers `*.g.cs` + classes `partial` ; le code manuel ne va jamais dans un `.g.cs`.
- Les couches ci-dessus **priment** sur la consigne « moins de code » de Ponytail : Ponytail s'applique *à l'intérieur* d'une couche (pas d'abstraction spéculative, pas de dépendance inutile), jamais pour fusionner des couches.

## Conventions

- Base : `snake_case`, un schéma PostgreSQL par module, PK `uuid` (`uuidv7()`), colonnes d'audit, suppression logique, concurrence `xmin`. Le schéma EF doit reproduire `database/01-schema.sql`.
- C# : `PascalCase`, `nullable enable`, `TreatWarningsAsErrors` sur les nouveaux projets, `async` suffixé `Async`, un type par fichier. Langue du domaine : français (`Projet`, `Phase`, `Tache`, `Ticket`).
- Tests : xUnit + FluentAssertions ; bUnit pour les composants ; NetArchTest pour les dépendances de couches ; PostgreSQL de test via `python scripts/pg-test.py demarrer|arreter` (instance jetable, port 55432, sans mot de passe).
- Secrets : jamais dans le dépôt ni dans un fichier suivi. Base de dev : variable `ConnectionStrings__Gp` (déjà fournie à tes commandes) ; ne jamais afficher, journaliser, committer ni recopier cette chaîne. Pipeline : variables GitLab CI masquées.
- Base `gprojet` = développement (migrations, `dotnet run`) ; **tests = instance jetable `pg-test` uniquement**, jamais `gprojet`.
- Git : branche `phase/Pn-<slug>` par phase ; **un commit par tâche** : `Pn-Tnn: <titre>` ; pas de push (l'utilisateur pousse après validation).

## Plugins et skills

| Outil | Quand l'utiliser |
|---|---|
| **graphify** | Dès qu'il y a du code : `graphify query "…"` avant d'explorer ; `/graphify .` en fin de phase pour rafraîchir le graphe (`graphify-out/`). |
| **ponytail** | Toujours actif : réutiliser l'existant et la BCL avant d'écrire du code, dans le respect des couches. `ponytail-review` avant chaque commit de tâche sonnet/opus. |
| **ui-ux-pro-max** | Avant toute page : ergonomie (hiérarchie, densité, parcours, états vide / chargement / erreur). **Jamais** pour la palette, la police ou le style : c'est la charte HCP qui fait foi. |
| **motion-design** | Où placer les mouvements (apparitions, dialogues, repli du menu), avec **les seules durées de la charte** (0.15s, 0.25s, 0.6s), en CSS, neutralisés sous `prefers-reduced-motion`. |
| **Sous-agents** | `executeur-opus`, `executeur-sonnet`, `executeur-haiku`, `verificateur` (voir ci-dessus). |
| Skills projet | `executer-tache`, `reprendre`, `conventions-backend`, `conventions-frontend`. |

## À ne jamais faire

`git push`, `reset --hard`, `--no-verify` ; écrire hors du projet (sauf `../Documentation`) ; mettre un secret en clair ; sauter une tâche ou en démarrer deux à la fois ; marquer une tâche `A_VALIDER` sans avoir exécuté son test ; lancer ou valider une phase.
