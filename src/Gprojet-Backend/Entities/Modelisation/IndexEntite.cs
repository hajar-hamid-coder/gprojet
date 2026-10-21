namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.index_entite.</summary>
public class IndexEntite : EntiteAuditable
{
    public Guid EntiteId { get; set; }

    public string Nom { get; set; } = null!;

    public bool EstUnique { get; set; }

    public Entite Entite { get; set; } = null!;

    public ICollection<IndexAttribut> IndexAttributs { get; } = [];
}
