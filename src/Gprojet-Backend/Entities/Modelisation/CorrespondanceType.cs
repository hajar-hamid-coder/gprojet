namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.correspondance_type.</summary>
public class CorrespondanceType : EntiteAuditable
{
    public string TypeLogique { get; set; } = null!;

    public CibleCorrespondance Cible { get; set; }

    public string TypePhysique { get; set; } = null!;
}
