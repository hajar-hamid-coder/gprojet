namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.type_ticket.</summary>
public class TypeTicket : EntiteAuditable
{
    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public string? Icone { get; set; }

    public Guid WorkflowId { get; set; }

    public bool Actif { get; set; } = true;

    public Workflow Workflow { get; set; } = null!;
}
