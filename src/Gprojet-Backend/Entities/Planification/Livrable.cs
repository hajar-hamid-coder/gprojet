namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.livrable.</summary>
public class Livrable : EntiteAuditable
{
    public Guid PhaseId { get; set; }

    public string Libelle { get; set; } = null!;

    public string? Description { get; set; }

    public StatutLivrable Statut { get; set; } = StatutLivrable.AProduire;

    public DateOnly? DatePrevue { get; set; }

    public DateOnly? DateLivraison { get; set; }

    public Phase Phase { get; set; } = null!;
}
