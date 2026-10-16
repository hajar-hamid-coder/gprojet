namespace Gprojet.Backend.Entities;

/// <summary>
/// Base de toutes les entités (sauf <c>Historique</c>) : PK uuid v7, colonnes d'audit, suppression logique
/// et jeton de concurrence porté par la colonne système <c>xmin</c>.
/// </summary>
public abstract class EntiteAuditable
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public DateTimeOffset CreeLe { get; set; }

    public Guid? CreePar { get; set; }

    public DateTimeOffset? ModifieLe { get; set; }

    public Guid? ModifiePar { get; set; }

    public bool EstSupprime { get; set; }

    public DateTimeOffset? SupprimeLe { get; set; }

    public Guid? SupprimePar { get; set; }

    /// <summary>Colonne système <c>xmin</c> (concurrence optimiste) ; « Version » est déjà une colonne métier de certaines tables.</summary>
    public uint VersionLigne { get; set; }
}
