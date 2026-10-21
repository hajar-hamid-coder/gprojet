namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.transition.</summary>
public class Transition : EntiteAuditable
{
    public Guid WorkflowId { get; set; }

    public Guid StatutDepuisId { get; set; }

    public Guid StatutVersId { get; set; }

    public string? Libelle { get; set; }

    public List<string> RolesAutorises { get; set; } = [];

    public Workflow Workflow { get; set; } = null!;

    public StatutWorkflow StatutDepuis { get; set; } = null!;

    public StatutWorkflow StatutVers { get; set; } = null!;
}
