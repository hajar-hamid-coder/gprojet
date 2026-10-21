namespace Gprojet.Backend.Entities.Projets;

/// <summary>Table projet.type_projet.</summary>
public class TypeProjet : EntiteAuditable
{
    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public string? Description { get; set; }

    public bool Actif { get; set; } = true;

    public ICollection<ModelePhase> ModelePhases { get; } = [];
}
