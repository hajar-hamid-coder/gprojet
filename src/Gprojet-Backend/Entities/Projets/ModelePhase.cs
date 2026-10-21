namespace Gprojet.Backend.Entities.Projets;

/// <summary>Table projet.modele_phase.</summary>
public class ModelePhase : EntiteAuditable
{
    public Guid TypeProjetId { get; set; }

    public short Ordre { get; set; }

    public string Nom { get; set; } = null!;

    public string? Objectif { get; set; }

    public string? CriteresEntree { get; set; }

    public string? CriteresSortie { get; set; }

    public ModeEnchainement ModeEnchainement { get; set; } = ModeEnchainement.Sequentiel;

    public decimal? PctMinChevauchement { get; set; }

    public TypeProjet TypeProjet { get; set; } = null!;
}
