namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.diagramme.</summary>
public class Diagramme : EntiteAuditable
{
    public Guid ModeleId { get; set; }

    public TypeDiagramme Type { get; set; }

    public string Nom { get; set; } = null!;

    public string? Contenu { get; set; }

    public string? SourceTexte { get; set; }

    public StatutValidationDiagramme StatutValidation { get; set; } = StatutValidationDiagramme.Brouillon;

    public int Version { get; set; } = 1;

    public ModeleDomaine Modele { get; set; } = null!;

    public ICollection<ElementDiagramme> ElementDiagrammes { get; } = [];
}
