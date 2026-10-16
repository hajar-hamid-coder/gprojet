namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.statut_workflow.</summary>
public class StatutWorkflow : EntiteAuditable
{
    public Guid WorkflowId { get; set; }

    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public CategorieStatut Categorie { get; set; }

    public string? Couleur { get; set; }

    public short Ordre { get; set; }

    public bool EstInitial { get; set; }

    public Workflow Workflow { get; set; } = null!;
}
