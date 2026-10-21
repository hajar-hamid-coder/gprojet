namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.workflow.</summary>
public class Workflow : EntiteAuditable
{
    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public TypeElementWorkflow TypeElement { get; set; }

    public bool EstDefaut { get; set; }

    public ICollection<StatutWorkflow> StatutWorkflows { get; } = [];

    public ICollection<Transition> Transitions { get; } = [];
}
