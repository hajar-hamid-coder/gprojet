namespace Gprojet.Backend.Entities.Organisation;

/// <summary>Table org.jour_ferie.</summary>
public class JourFerie : EntiteAuditable
{
    public DateOnly DateFerie { get; set; }

    public string Libelle { get; set; } = null!;
}
