using Gprojet.Backend.Entities.Planification;

namespace Gprojet.Backend.Entities.Projets;

/// <summary>Table projet.jalon.</summary>
public class Jalon : EntiteAuditable
{
    public Guid ProjetId { get; set; }

    public Guid? PhaseId { get; set; }

    public string Libelle { get; set; } = null!;

    public DateOnly DatePrevue { get; set; }

    public DateOnly? DateReelle { get; set; }

    public StatutJalon Statut { get; set; } = StatutJalon.AVenir;

    public Projet Projet { get; set; } = null!;

    public Phase? Phase { get; set; }
}
