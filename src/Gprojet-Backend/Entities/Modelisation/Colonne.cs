namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.colonne.</summary>
public class Colonne : EntiteAuditable
{
    public Guid TableId { get; set; }

    public Guid? AttributSourceId { get; set; }

    public string Nom { get; set; } = null!;

    public string TypePhysique { get; set; } = null!;

    public bool EstNullable { get; set; } = true;

    public bool EstClePrimaire { get; set; }

    public string? ValeurDefaut { get; set; }

    public short Ordre { get; set; } = 1;

    public bool EstSurchargee { get; set; }

    public TableBd Table { get; set; } = null!;

    public Attribut? AttributSource { get; set; }
}
