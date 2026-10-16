using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Entities.Commun;

/// <summary>Table commun.notification.</summary>
public class Notification : EntiteAuditable
{
    public Guid DestinataireId { get; set; }

    public string Type { get; set; } = null!;

    public string Titre { get; set; } = null!;

    public string? Message { get; set; }

    public string? Lien { get; set; }

    public bool EstLue { get; set; }

    public DateTimeOffset? DateLecture { get; set; }

    public Agent Destinataire { get; set; } = null!;
}
