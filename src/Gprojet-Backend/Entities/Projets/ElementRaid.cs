using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Entities.Projets;

/// <summary>Table projet.element_raid.</summary>
public class ElementRaid : EntiteAuditable
{
    public Guid ProjetId { get; set; }

    public TypeElementRaid Type { get; set; }

    public string Titre { get; set; } = null!;

    public string? Description { get; set; }

    public short? Probabilite { get; set; }

    public short? Impact { get; set; }

    /// <summary>Colonne calculée (GENERATED ALWAYS AS … STORED) : lecture seule.</summary>
    public short? Criticite { get; private set; }

    public StatutElementRaid Statut { get; set; } = StatutElementRaid.Ouvert;

    public string? PlanAction { get; set; }

    public DateOnly? Echeance { get; set; }

    public Guid? ResponsableId { get; set; }

    public Projet Projet { get; set; } = null!;

    public Agent? Responsable { get; set; }
}
