namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.valeur_enumeration.</summary>
public class ValeurEnumeration : EntiteAuditable
{
    public Guid EnumerationId { get; set; }

    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public int? Valeur { get; set; }

    public short Ordre { get; set; } = 1;

    public Enumeration Enumeration { get; set; } = null!;
}
