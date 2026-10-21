using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Entities.Projets;

/// <summary>Table projet.membre_projet.</summary>
public class MembreProjet : EntiteAuditable
{
    public Guid ProjetId { get; set; }

    public Guid AgentId { get; set; }

    public RoleProjet RoleProjet { get; set; } = RoleProjet.Membre;

    public decimal QuotaPct { get; set; } = 100m;

    public DateOnly? DateDebut { get; set; }

    public DateOnly? DateFin { get; set; }

    public Guid? AffecteParId { get; set; }

    public Projet Projet { get; set; } = null!;

    public Agent Agent { get; set; } = null!;

    public Agent? AffectePar { get; set; }
}
