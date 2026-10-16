namespace Gprojet.Backend.Entities.Temps;

/// <summary>Table temps.categorie_activite.</summary>
public class CategorieActivite : EntiteAuditable
{
    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public bool EstProjet { get; set; } = true;

    public bool Actif { get; set; } = true;
}
