namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.parametre_methode.</summary>
public class ParametreMethode : EntiteAuditable
{
    public Guid MethodeId { get; set; }

    public string Nom { get; set; } = null!;

    public string Type { get; set; } = null!;

    public short Ordre { get; set; } = 1;

    public bool EstOptionnel { get; set; }

    public string? ValeurDefaut { get; set; }

    public Methode Methode { get; set; } = null!;
}
