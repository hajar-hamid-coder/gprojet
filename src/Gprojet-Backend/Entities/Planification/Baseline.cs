using Gprojet.Backend.Entities.Projets;

namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.baseline.</summary>
public class Baseline : EntiteAuditable
{
    public Guid ProjetId { get; set; }

    public string Libelle { get; set; } = null!;

    public DateTimeOffset DateCreation { get; set; }

    public Projet Projet { get; set; } = null!;

    public ICollection<BaselineTache> BaselineTaches { get; } = [];
}
