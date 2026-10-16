namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.element_diagramme.</summary>
public class ElementDiagramme : EntiteAuditable
{
    public Guid DiagrammeId { get; set; }

    public TypeElementDiagramme TypeElement { get; set; }

    public Guid ElementId { get; set; }

    public decimal X { get; set; }

    public decimal Y { get; set; }

    public decimal? Largeur { get; set; }

    public decimal? Hauteur { get; set; }

    public string? Style { get; set; }

    public Diagramme Diagramme { get; set; } = null!;
}
