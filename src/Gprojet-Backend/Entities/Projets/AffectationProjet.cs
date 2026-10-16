using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Entities.Projets;

/// <summary>Table projet.affectation_projet.</summary>
public class AffectationProjet : EntiteAuditable
{
    public Guid ProjetId { get; set; }

    public Guid? PrecedenteId { get; set; }

    public Guid EntiteSourceId { get; set; }

    public Guid EntiteCibleId { get; set; }

    public NiveauCible NiveauCible { get; set; }

    public StatutAffectation Statut { get; set; } = StatutAffectation.EnAttente;

    public string? Instructions { get; set; }

    public DateOnly? EcheanceSouhaitee { get; set; }

    public decimal? ChargePrevueJh { get; set; }

    public Guid AffecteParId { get; set; }

    public DateTimeOffset DateAffectation { get; set; }

    public Guid? ReponduParId { get; set; }

    public DateTimeOffset? DateReponse { get; set; }

    public string? MotifReponse { get; set; }

    public bool EstActive { get; set; } = true;

    public Projet Projet { get; set; } = null!;

    public AffectationProjet? Precedente { get; set; }

    public EntiteOrganisationnelle EntiteSource { get; set; } = null!;

    public EntiteOrganisationnelle EntiteCible { get; set; } = null!;

    public Agent AffectePar { get; set; } = null!;

    public Agent? ReponduPar { get; set; }
}
