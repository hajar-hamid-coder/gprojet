namespace Gprojet.Backend.Entities.Organisation;

/// <summary>Table org.absence.</summary>
public class Absence : EntiteAuditable
{
    public Guid AgentId { get; set; }

    public DateOnly DateDebut { get; set; }

    public DateOnly DateFin { get; set; }

    public TypeAbsence Type { get; set; }

    public bool DemiJournee { get; set; }

    public string? Commentaire { get; set; }

    public Agent Agent { get; set; } = null!;
}
