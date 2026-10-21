namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.association.</summary>
public class Association : EntiteAuditable
{
    public Guid ModeleId { get; set; }

    public string Nom { get; set; } = null!;

    public string NomTechnique { get; set; } = null!;

    public NatureAssociation Nature { get; set; } = NatureAssociation.Association;

    public RegleSuppression RegleSuppression { get; set; } = RegleSuppression.Restrict;

    public string? Description { get; set; }

    public ModeleDomaine Modele { get; set; } = null!;

    public ICollection<Patte> Pattes { get; } = [];

    public ICollection<Attribut> Attributs { get; } = [];
}
