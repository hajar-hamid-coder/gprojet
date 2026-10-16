using Gprojet.Backend.Entities.Organisation;
using Gprojet.Backend.Entities.Projets;

namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.ticket.</summary>
public class Ticket : EntiteAuditable
{
    public Guid TacheId { get; set; }

    public Guid ProjetId { get; set; }

    public string Numero { get; set; } = null!;

    public Guid TypeTicketId { get; set; }

    public Guid StatutId { get; set; }

    public string Titre { get; set; } = null!;

    public string? Description { get; set; }

    public Priorite Priorite { get; set; } = Priorite.Normale;

    public Severite? Severite { get; set; }

    public int Rang { get; set; }

    public bool EstBloquant { get; set; }

    public string? Environnement { get; set; }

    public string? EtapesReproduction { get; set; }

    public string? CriteresAcceptation { get; set; }

    public decimal? EstimationH { get; set; }

    public decimal? ResteAFaireH { get; set; }

    public DateOnly? DateEcheance { get; set; }

    public Guid? AssigneId { get; set; }

    public Guid RapporteurId { get; set; }

    public Tache Tache { get; set; } = null!;

    public Projet Projet { get; set; } = null!;

    public TypeTicket TypeTicket { get; set; } = null!;

    public StatutWorkflow Statut { get; set; } = null!;

    public Agent? Assigne { get; set; }

    public Agent Rapporteur { get; set; } = null!;
}
