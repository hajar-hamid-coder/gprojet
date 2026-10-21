namespace Gprojet.Backend.Entities.Organisation;

/// <summary>Table org.delegation.</summary>
public class Delegation : EntiteAuditable
{
    public Guid DelegantId { get; set; }

    public Guid DelegataireId { get; set; }

    public Guid EntiteId { get; set; }

    public DateOnly DateDebut { get; set; }

    public DateOnly DateFin { get; set; }

    public string? Motif { get; set; }

    public bool PeutCreerProjet { get; set; }

    public bool PeutAffecter { get; set; }

    public bool PeutValider { get; set; }

    public Agent Delegant { get; set; } = null!;

    public Agent Delegataire { get; set; } = null!;

    public EntiteOrganisationnelle Entite { get; set; } = null!;
}
