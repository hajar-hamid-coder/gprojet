using Gprojet.Backend.Entities.Projets;

namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.config_generation.</summary>
public class ConfigGeneration : EntiteAuditable
{
    public Guid ProjetId { get; set; }

    public StackBack StackBack { get; set; } = StackBack.Dotnet10;

    public StackFront StackFront { get; set; } = StackFront.BlazorWasm;

    public ArchitectureGeneration Architecture { get; set; } = ArchitectureGeneration.CouchesGp;

    public string? DepotGitlabId { get; set; }

    public string BrancheCible { get; set; } = "generation/socle";

    public Projet Projet { get; set; } = null!;

    public ICollection<TemplateGeneration> TemplateGenerations { get; } = [];
}
