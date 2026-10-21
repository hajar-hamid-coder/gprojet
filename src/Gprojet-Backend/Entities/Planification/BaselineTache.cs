namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.baseline_tache.</summary>
public class BaselineTache : EntiteAuditable
{
    public Guid BaselineId { get; set; }

    public Guid TacheId { get; set; }

    public DateOnly? DateDebut { get; set; }

    public DateOnly? DateFin { get; set; }

    public decimal? ChargeH { get; set; }

    public Baseline Baseline { get; set; } = null!;

    public Tache Tache { get; set; } = null!;
}
