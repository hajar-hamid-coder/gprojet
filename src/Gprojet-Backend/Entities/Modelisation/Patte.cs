namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.patte.</summary>
public class Patte : EntiteAuditable
{
    public Guid AssociationId { get; set; }

    public Guid EntiteId { get; set; }

    public short CardMin { get; set; }

    public CardinaliteMax CardMax { get; set; }

    public string? Role { get; set; }

    public string? NomNavigation { get; set; }

    public bool EstNavigable { get; set; } = true;

    public short Ordre { get; set; } = 1;

    public Association Association { get; set; } = null!;

    public Entite Entite { get; set; } = null!;
}
