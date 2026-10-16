using Gprojet.Backend.Entities.Modelisation;
using Gprojet.Backend.Entities.Organisation;
using Gprojet.Backend.Entities.Planification;

namespace Gprojet.Backend.Entities.Projets;

/// <summary>Table projet.projet.</summary>
public class Projet : EntiteAuditable
{
    public string Code { get; set; } = null!;

    public string Intitule { get; set; } = null!;

    public string? Description { get; set; }

    public string? Objectifs { get; set; }

    public string? Perimetre { get; set; }

    public Guid TypeProjetId { get; set; }

    public Priorite Priorite { get; set; } = Priorite.Normale;

    public StatutProjet Statut { get; set; } = StatutProjet.Brouillon;

    public Meteo Meteo { get; set; } = Meteo.Vert;

    public Classification Classification { get; set; } = Classification.Interne;

    public ModeOrdonnancement ModeOrdonnancement { get; set; } = ModeOrdonnancement.Bloquant;

    public Guid CreateurId { get; set; }

    public Guid EntiteCreatriceId { get; set; }

    public Guid? EntitePorteuseId { get; set; }

    public Guid? ChefProjetId { get; set; }

    public string CheminEntite { get; set; } = null!;

    public DateOnly? DateDebutPrevue { get; set; }

    public DateOnly? DateFinPrevue { get; set; }

    public DateOnly? DateDebutReelle { get; set; }

    public DateOnly? DateFinReelle { get; set; }

    public decimal ChargeEstimeeJh { get; set; }

    public decimal ChargeConsommeeJh { get; set; }

    public decimal AvancementPct { get; set; }

    public int ProchainNumeroTicket { get; set; } = 1;

    public TypeProjet TypeProjet { get; set; } = null!;

    public Agent Createur { get; set; } = null!;

    public EntiteOrganisationnelle EntiteCreatrice { get; set; } = null!;

    public EntiteOrganisationnelle? EntitePorteuse { get; set; }

    public Agent? ChefProjet { get; set; }

    public ICollection<AffectationProjet> AffectationProjets { get; } = [];

    public ICollection<MembreProjet> MembreProjets { get; } = [];

    public ICollection<Jalon> Jalons { get; } = [];

    public ICollection<ElementRaid> ElementRaids { get; } = [];

    public ICollection<Phase> Phases { get; } = [];

    public ICollection<Tache> Taches { get; } = [];

    public ICollection<Ticket> Tickets { get; } = [];

    public ICollection<Baseline> Baselines { get; } = [];

    public ICollection<ModeleDomaine> ModeleDomaines { get; } = [];

    public ICollection<ConfigGeneration> ConfigGenerations { get; } = [];
}
