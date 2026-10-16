namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.index_attribut.</summary>
public class IndexAttribut : EntiteAuditable
{
    public Guid IndexId { get; set; }

    public Guid AttributId { get; set; }

    public short Ordre { get; set; } = 1;

    public IndexEntite Index { get; set; } = null!;

    public Attribut Attribut { get; set; } = null!;
}
