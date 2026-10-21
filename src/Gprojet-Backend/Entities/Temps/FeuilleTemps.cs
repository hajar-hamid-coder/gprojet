using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Entities.Temps;

/// <summary>Table temps.feuille_temps.</summary>
public class FeuilleTemps : EntiteAuditable
{
    public Guid AgentId { get; set; }

    public DateOnly SemaineDebut { get; set; }

    public StatutFeuilleTemps Statut { get; set; } = StatutFeuilleTemps.Brouillon;

    public DateTimeOffset? DateSoumission { get; set; }

    public Guid? ValideurId { get; set; }

    public DateTimeOffset? DateValidation { get; set; }

    public string? MotifRejet { get; set; }

    public Agent Agent { get; set; } = null!;

    public Agent? Valideur { get; set; }

    public ICollection<SaisieTemps> SaisieTemps { get; } = [];
}
