namespace Gprojet.Backend.Entities.Organisation;

/// <summary>Table org.fonction.</summary>
public class Fonction : EntiteAuditable
{
    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;
}
