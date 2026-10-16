using Gprojet.Backend.Entities.Commun;

namespace Gprojet.Backend.Entities.Organisation;

/// <summary>Table org.agent.</summary>
public class Agent : EntiteAuditable
{
    public string Matricule { get; set; } = null!;

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telephone { get; set; }

    public Guid EntiteId { get; set; }

    public Guid? FonctionId { get; set; }

    public decimal CapaciteHebdoH { get; set; } = 40m;

    public string? KeycloakSub { get; set; }

    public bool Actif { get; set; } = true;

    public EntiteOrganisationnelle Entite { get; set; } = null!;

    public Fonction? Fonction { get; set; }

    public ICollection<Rattachement> Rattachements { get; } = [];

    public ICollection<AgentCompetence> AgentCompetences { get; } = [];

    public ICollection<Absence> Absences { get; } = [];

    public ICollection<Notification> Notifications { get; } = [];
}
