using Gprojet.Backend.Entities.Projets;

namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.phase.</summary>
public class Phase : EntiteAuditable
{
    public Guid ProjetId { get; set; }

    public string? Code { get; set; }

    public string Nom { get; set; } = null!;

    public string? Objectif { get; set; }

    public short Ordre { get; set; }

    public StatutPhase Statut { get; set; } = StatutPhase.AVenir;

    public ModeEnchainement ModeEnchainement { get; set; } = ModeEnchainement.Sequentiel;

    public decimal? PctMinChevauchement { get; set; }

    public string? CriteresEntree { get; set; }

    public string? CriteresSortie { get; set; }

    public DateOnly? DateDebutPrevue { get; set; }

    public DateOnly? DateFinPrevue { get; set; }

    public DateOnly? DateDebutReelle { get; set; }

    public DateOnly? DateFinReelle { get; set; }

    public decimal ChargeEstimeeH { get; set; }

    public decimal AvancementPct { get; set; }

    public Projet Projet { get; set; } = null!;

    public ICollection<ValidationPhase> ValidationPhases { get; } = [];

    public ICollection<Livrable> Livrables { get; } = [];

    public ICollection<Tache> Taches { get; } = [];
}
