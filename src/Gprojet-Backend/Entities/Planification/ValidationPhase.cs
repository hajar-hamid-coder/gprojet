using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.validation_phase.</summary>
public class ValidationPhase : EntiteAuditable
{
    public Guid PhaseId { get; set; }

    public Guid ValideurId { get; set; }

    public short Tour { get; set; } = 1;

    public short OrdreNiveau { get; set; }

    public DecisionValidation Decision { get; set; } = DecisionValidation.EnAttente;

    public string? Commentaire { get; set; }

    public DateTimeOffset? DateDecision { get; set; }

    public Phase Phase { get; set; } = null!;

    public Agent Valideur { get; set; } = null!;
}
