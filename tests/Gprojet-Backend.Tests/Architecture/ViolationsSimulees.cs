// Violations volontaires pour le test garde-fou : elles reproduisent la hiérarchie des couches
// sous le préfixe Gprojet.Backend.Tests.Architecture.Violations, dans le projet de test uniquement
// (jamais dans l'assembly backend). Plusieurs types par fichier : exception assumée, c'est un jeu d'essai.
#pragma warning disable CA1812, CA1822, IDE0051, IDE0052

namespace Gprojet.Backend.Tests.Architecture.Violations.Common
{
    public sealed class Horodatage;
}

namespace Gprojet.Backend.Tests.Architecture.Violations.Entities
{
    public sealed class Projet
    {
        public Common.Horodatage? Cree { get; set; } // autorisé : Entities -> Common
    }

    // Entities -> Services : interdit
    public sealed class EntiteQuiAppelleUnService
    {
        public Services.ProjetService? Service { get; set; }
    }
}

namespace Gprojet.Backend.Tests.Architecture.Violations.Repositories
{
    public interface IProjetRepository
    {
        Entities.Projet? Trouver();
    }

    public sealed class ProjetRepository : IProjetRepository
    {
        public Entities.Projet? Trouver() => null;
    }

    // Repositories -> Contracts et Mapping : interdit (les DTO ne descendent pas sous l'Orchestration)
    public sealed class RepositoryQuiRenvoieUnContrat
    {
        public Contracts.ProjetDto? Lire() => null;

        public Mapping.ProjetMapper? Mapper { get; set; }
    }
}

namespace Gprojet.Backend.Tests.Architecture.Violations.Contracts
{
    using FluentValidation;

    public sealed record ProjetDto(Guid Id, string Nom);

    // autorisé : BCL + FluentValidation + Contracts
    public sealed class ProjetDtoValidator : AbstractValidator<ProjetDto>
    {
        public ProjetDtoValidator() => RuleFor(x => x.Nom).NotEmpty();
    }

    // Contracts -> Entities : interdit (et hors liste blanche)
    public sealed record ContratQuiExposeUneEntite(Entities.Projet Projet);

    // Contracts -> ASP.NET Core : hors liste blanche
    public sealed record ContratQuiDependDAspNet(Microsoft.AspNetCore.Http.IFormFile Fichier);
}

namespace Gprojet.Backend.Tests.Architecture.Violations.Mapping
{
    public sealed class ProjetMapper
    {
        public Contracts.ProjetDto Vers(Entities.Projet projet) => new(Guid.Empty, projet.ToString()!);
    }
}

namespace Gprojet.Backend.Tests.Architecture.Violations.Services
{
    public sealed class ProjetService
    {
        public void Cloturer(Entities.Projet projet) => _ = projet; // autorisé : Services -> Entities
    }

    // Services -> Contracts : interdit
    public sealed class ServiceQuiConnaitUnContrat
    {
        public Contracts.ProjetDto? Dto { get; set; }
    }

    // Services -> Mapping : interdit
    public sealed class ServiceQuiMappe
    {
        public Mapping.ProjetMapper? Mapper { get; set; }
    }

    // Services -> EF Core : interdit
    public sealed class ServiceQuiUtiliseEfCore
    {
        public Microsoft.EntityFrameworkCore.DbContext? Contexte { get; set; }
    }

    // Dto hors de Contracts : interdit
    public sealed record ProjetEgareDto(Guid Id);

    // Contrôleur hors de Web, détecté par héritage (le nom ne finit pas par Controller)
    public sealed class PointDEntreeEgare : Microsoft.AspNetCore.Mvc.ControllerBase;
}

namespace Gprojet.Backend.Tests.Architecture.Violations.Orchestration
{
    public sealed class OrchestrateurCorrect(Repositories.IProjetRepository repository)
    {
        public Entities.Projet? Lire() => repository.Trouver(); // autorisé : interface de repository
    }

    // Orchestration -> implémentation concrète de repository : interdit
    public sealed class OrchestrateurQuiInstancieLeRepository
    {
        public Entities.Projet? Lire() => new Repositories.ProjetRepository().Trouver();
    }

    // Contrôleur hors de Web, détecté par son nom
    public sealed class ProjetsController;
}

namespace Gprojet.Backend.Tests.Architecture.Violations.Web
{
    public sealed class ControleurCorrect : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        public Contracts.ProjetDto? Dernier { get; set; } // autorisé : Web -> Contracts
    }

    // Web -> Services : interdit
    public sealed class WebQuiAppelleUnService
    {
        public Services.ProjetService? Service { get; set; }
    }

    // Web -> Repositories : interdit
    public sealed class WebQuiAppelleUnRepository
    {
        public Repositories.IProjetRepository? Repository { get; set; }
    }

    // Web -> Entities : interdit
    public sealed class WebQuiExposeUneEntite
    {
        public Entities.Projet? Projet { get; set; }
    }
}
