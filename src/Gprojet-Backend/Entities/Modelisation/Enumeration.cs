namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.enumeration.</summary>
public class Enumeration : EntiteAuditable
{
    public Guid ModuleId { get; set; }

    public string Nom { get; set; } = null!;

    public string NomTechnique { get; set; } = null!;

    public StockageEnumeration Stockage { get; set; } = StockageEnumeration.Texte;

    public string? Description { get; set; }

    public Module Module { get; set; } = null!;

    public ICollection<ValeurEnumeration> ValeurEnumerations { get; } = [];
}
