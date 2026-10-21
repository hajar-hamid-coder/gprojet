using Gprojet.Backend.Entities.Organisation;
using Gprojet.Backend.Entities.Projets;

namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.tache.</summary>
public class Tache : EntiteAuditable
{
    public Guid PhaseId { get; set; }

    public Guid ProjetId { get; set; }

    public string CodeWbs { get; set; } = null!;

    public string Titre { get; set; } = null!;

    public string? Description { get; set; }

    public short Ordre { get; set; }

    public ModeExecution ModeExecution { get; set; } = ModeExecution.Sequentielle;

    public Priorite Priorite { get; set; } = Priorite.Normale;

    public Guid StatutId { get; set; }

    public Guid? ResponsableId { get; set; }

    public DateOnly? DateDebutPrevue { get; set; }

    public DateOnly? DateFinPrevue { get; set; }

    public DateOnly? DateDebutReelle { get; set; }

    public DateOnly? DateFinReelle { get; set; }

    public decimal? ChargeEstimeeH { get; set; }

    public decimal? ChargeOptimisteH { get; set; }

    public decimal? ChargePessimisteH { get; set; }

    public decimal? ResteAFaireH { get; set; }

    public decimal AvancementPct { get; set; }

    public decimal? MargeTotaleJ { get; set; }

    public decimal? MargeLibreJ { get; set; }

    public bool EstCritique { get; set; }

    public bool EstRecurrente { get; set; }

    public string? RegleRecurrence { get; set; }

    public Phase Phase { get; set; } = null!;

    public Projet Projet { get; set; } = null!;

    public StatutWorkflow Statut { get; set; } = null!;

    public Agent? Responsable { get; set; }

    public ICollection<Ticket> Tickets { get; } = [];

    public ICollection<BaselineTache> BaselineTaches { get; } = [];
}
