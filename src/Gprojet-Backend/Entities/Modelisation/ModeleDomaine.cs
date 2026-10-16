using Gprojet.Backend.Entities.Projets;

namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.modele_domaine.</summary>
public class ModeleDomaine : EntiteAuditable
{
    public Guid ProjetId { get; set; }

    public string Nom { get; set; } = null!;

    public int Version { get; set; } = 1;

    public StatutModeleDomaine Statut { get; set; } = StatutModeleDomaine.Brouillon;

    public string NamespaceRacine { get; set; } = null!;

    public SgbdCible SgbdCible { get; set; } = SgbdCible.Postgresql;

    public string? Description { get; set; }

    public Projet Projet { get; set; } = null!;

    public ICollection<Module> Modules { get; } = [];

    public ICollection<Association> Associations { get; } = [];

    public ICollection<TableBd> TableBds { get; } = [];

    public ICollection<Diagramme> Diagrammes { get; } = [];

    public ICollection<ExecutionGeneration> ExecutionGenerations { get; } = [];
}
