namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.entite_interface.</summary>
public class EntiteInterface : EntiteAuditable
{
    public Guid EntiteId { get; set; }

    public Guid InterfaceId { get; set; }

    public Entite Entite { get; set; } = null!;

    public Entite Interface { get; set; } = null!;
}
