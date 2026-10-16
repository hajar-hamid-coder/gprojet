---
name: conventions-backend
description: Conventions de code du backend GestionProjet (projet unique Gprojet-Backend, couches en dossiers/espaces de noms, EF Core/PostgreSQL, Mapperly, FluentValidation, périmètre hiérarchique, BFF). À charger avant d'écrire ou de relire du code C# côté serveur.
paths: "src/Gprojet-Backend/**, tests/Gprojet-Backend.Tests/**"
---

# Conventions backend

**Un seul projet** : `src/Gprojet-Backend/Gprojet-Backend.csproj` (`RootNamespace Gprojet.Backend`). Les couches sont des **dossiers** dont l'espace de noms suit le chemin (`Gprojet.Backend.Services.Projets`…). Ne jamais créer d'autre projet applicatif.

## Placement d'un cas d'usage (exemple : affecter un projet à une division)

| Couche | Fichier | Contenu |
|---|---|---|
| Contracts | `Contracts/Projets/AffecterProjetDto.cs` + `AffecterProjetDtoValidator.cs` | record DTO + règles FluentValidation (compilés aussi dans le front) |
| Web | `Web/Controllers/Projets/AffectationsController.cs` | route, `[Authorize(Policy=…)]`, appelle l'orchestrateur, renvoie `ActionResult<T>` |
| Orchestration | `Orchestration/Projets/AffectationOrchestrator.cs` | vérifie le périmètre, appelle les services, `IUnitOfWork.CommitAsync`, mappe vers DTO |
| Services | `Services/Projets/AffectationService.cs` | règles métier sur entités (cascade, statuts), lève `RegleMetierException` |
| Mapping | `Mapping/Projets/ProjetMapper.cs` | `[Mapper] public partial class` (Mapperly) |
| Repositories | `Repositories/Projets/ProjetRepository.cs` | requêtes EF par agrégat, `Include` explicites |
| Entities | `Entities/Projets/AffectationProjet.cs` | entité, invariants simples, navigation |

## Règles
- **Dépendances entre dossiers** : Web → Orchestration → Services → Repositories → Entities (+ Mapping, Contracts, Infrastructure, Common), exactement comme dans `../Documentation/02-architecture.md`. Le compilateur ne les vérifie plus : les tests NetArchTest de `tests/Gprojet-Backend.Tests/Architecture/` le font. Toute nouvelle couche ou exception = nouvelle règle de test.
- **Contracts** : uniquement BCL + FluentValidation (les fichiers sont liés dans `Gprojet-Front`, qui tourne dans le navigateur). Jamais d'EF Core, d'ASP.NET ni d'entité.
- Entités : héritent de `EntiteAuditable` (Id `Guid` v7, audit, suppression logique). Concurrence : `xmin` via `UseXminAsConcurrencyToken()`.
- Configurations EF : une classe `IEntityTypeConfiguration<T>` par entité dans `Repositories/Configurations/<Module>/`, noms `snake_case` explicites, schéma du module, index identiques à `database/01-schema.sql`. Migrations dans `Repositories/Migrations/`.
- Filtre global EF sur `est_supprime` ; filtre de périmètre via `IPerimetreCourant` sur Projet / Phase / Tâche / Ticket / Saisie.
- Services : pas de `DbContext`, pas de DTO. Interfaces `I<Nom>Service` à côté de l'implémentation.
- Orchestrateurs : une transaction par méthode publique ; aucun `try/catch` silencieux ; exceptions métier → `ProblemDetails` (400/403/404/409) dans le middleware global.
- Reporting : `Orchestration/Reporting/*Query.cs` en projection directe (`Select` vers DTO) ou SQL Dapper — pas de Repository/Mapper.
- BFF : endpoints `Web/Auth/` (`/bff/login`, `/bff/logout`, `/bff/user`), cookie `HttpOnly` + `Secure` + `SameSite=Strict`, anti-CSRF ; le backend sert le front (`UseBlazorFrameworkFiles`, `MapFallbackToFile`).
- Async partout (`CancellationToken` propagé). Pas de `.Result` / `.Wait()`.
- Journal : `ILogger<T>` structuré (Serilog), jamais de donnée personnelle complète ni de chaîne de connexion dans les logs.

## Tests attendus (`tests/Gprojet-Backend.Tests`)
- Règle métier → `Unitaires/` (cas nominal + chaque refus).
- Endpoint → `Integration/` sur l'instance `pg-test` (WebApplicationFactory), **jamais sur `gprojet`**.
- Dépendances entre couches → `Architecture/` (NetArchTest).
