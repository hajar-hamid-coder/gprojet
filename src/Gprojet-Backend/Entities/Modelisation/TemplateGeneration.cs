namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.template_generation.</summary>
public class TemplateGeneration : EntiteAuditable
{
    public Guid? ConfigId { get; set; }

    public CoucheTemplate Couche { get; set; }

    public PorteeTemplate Portee { get; set; } = PorteeTemplate.Entite;

    public string Nom { get; set; } = null!;

    public string CheminSortie { get; set; } = null!;

    public string Contenu { get; set; } = null!;

    public int Version { get; set; } = 1;

    public bool EstDefaut { get; set; }

    public ConfigGeneration? Config { get; set; }
}
