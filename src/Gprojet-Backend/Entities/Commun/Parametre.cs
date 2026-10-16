namespace Gprojet.Backend.Entities.Commun;

/// <summary>Table commun.parametre.</summary>
public class Parametre : EntiteAuditable
{
    public string Cle { get; set; } = null!;

    public string Valeur { get; set; } = null!;

    public TypeValeurParametre TypeValeur { get; set; }

    public string? Description { get; set; }
}
