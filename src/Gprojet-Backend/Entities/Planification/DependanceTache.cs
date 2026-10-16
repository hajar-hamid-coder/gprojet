namespace Gprojet.Backend.Entities.Planification;

/// <summary>Table planif.dependance_tache.</summary>
public class DependanceTache : EntiteAuditable
{
    public Guid PredecesseurId { get; set; }

    public Guid SuccesseurId { get; set; }

    public TypeLienDependance TypeLien { get; set; } = TypeLienDependance.FinDebut;

    public decimal DelaiJ { get; set; }

    public Tache Predecesseur { get; set; } = null!;

    public Tache Successeur { get; set; } = null!;
}
