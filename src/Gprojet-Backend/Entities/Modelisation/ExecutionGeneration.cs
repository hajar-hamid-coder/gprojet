using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.execution_generation.</summary>
public class ExecutionGeneration : EntiteAuditable
{
    public Guid ModeleId { get; set; }

    public int VersionModele { get; set; }

    public Guid LanceurId { get; set; }

    public DateTimeOffset DateExecution { get; set; }

    public StatutExecution Statut { get; set; } = StatutExecution.EnCours;

    public int? NbFichiers { get; set; }

    public string? Branche { get; set; }

    public string? CommitSha { get; set; }

    public string? Rapport { get; set; }

    public ModeleDomaine Modele { get; set; } = null!;

    public Agent Lanceur { get; set; } = null!;
}
