namespace Gprojet.Backend.Entities.Organisation;

/// <summary>Table org.entite_organisationnelle.</summary>
public class EntiteOrganisationnelle : EntiteAuditable
{
    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public string? Sigle { get; set; }

    public NiveauEntite Niveau { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? ResponsableId { get; set; }

    public string Chemin { get; set; } = null!;

    public bool Actif { get; set; } = true;

    public EntiteOrganisationnelle? Parent { get; set; }

    public Agent? Responsable { get; set; }
}
