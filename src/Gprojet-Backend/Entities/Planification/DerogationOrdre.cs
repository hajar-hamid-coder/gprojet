using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.derogation_ordre.</summary>
public class DerogationOrdre : EntiteAuditable
{
    public Guid TacheId { get; set; }

    public Guid PredecesseurId { get; set; }

    public Guid AgentId { get; set; }

    public string Justification { get; set; } = null!;

    public DateTimeOffset DateDerogation { get; set; }

    public Tache Tache { get; set; } = null!;

    public Tache Predecesseur { get; set; } = null!;

    public Agent Agent { get; set; } = null!;
}
