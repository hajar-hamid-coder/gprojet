namespace Gprojet.Backend.Entities.Organisation;

/// <summary>Table org.rattachement.</summary>
public class Rattachement : EntiteAuditable
{
    public Guid AgentId { get; set; }

    public Guid EntiteId { get; set; }

    public DateOnly DateDebut { get; set; }

    public DateOnly? DateFin { get; set; }

    public string? Motif { get; set; }

    public Agent Agent { get; set; } = null!;

    public EntiteOrganisationnelle Entite { get; set; } = null!;
}
