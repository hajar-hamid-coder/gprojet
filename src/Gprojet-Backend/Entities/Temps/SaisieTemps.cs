using Gprojet.Backend.Entities.Organisation;
using Gprojet.Backend.Entities.Planification;
using Gprojet.Backend.Entities.Projets;

namespace Gprojet.Backend.Entities.Temps;

/// <summary>Table temps.saisie_temps.</summary>
public class SaisieTemps : EntiteAuditable
{
    public Guid FeuilleId { get; set; }

    public Guid AgentId { get; set; }

    public DateOnly DateSaisie { get; set; }

    public decimal DureeH { get; set; }

    public Guid CategorieId { get; set; }

    public Guid? ProjetId { get; set; }

    public Guid? TacheId { get; set; }

    public Guid? TicketId { get; set; }

    public string? Commentaire { get; set; }

    public SourceSaisie Source { get; set; } = SourceSaisie.Manuel;

    public DateTimeOffset? ChronoDebut { get; set; }

    public DateTimeOffset? ChronoFin { get; set; }

    public string? ReferenceGitlab { get; set; }

    public FeuilleTemps Feuille { get; set; } = null!;

    public Agent Agent { get; set; } = null!;

    public CategorieActivite Categorie { get; set; } = null!;

    public Projet? Projet { get; set; }

    public Tache? Tache { get; set; }

    public Ticket? Ticket { get; set; }
}
