namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.lien_ticket.</summary>
public class LienTicket : EntiteAuditable
{
    public Guid SourceId { get; set; }

    public Guid CibleId { get; set; }

    public TypeLienTicket TypeLien { get; set; }

    public Ticket Source { get; set; } = null!;

    public Ticket Cible { get; set; } = null!;
}
