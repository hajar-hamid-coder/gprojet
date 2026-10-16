namespace Gprojet.Backend.Repositories;

/// <summary>Unité de travail d'un cas d'usage : enregistre les changements et regroupe plusieurs enregistrements dans une transaction.</summary>
public interface IUnitOfWork
{
    /// <summary>Enregistre les changements suivis ; une modification concurrente de la même ligne lève <c>ConflitException</c>.</summary>
    Task<int> CommitAsync(CancellationToken ct = default);

    /// <summary>
    /// Exécute <paramref name="action"/> dans une transaction : validée si elle se termine, annulée si elle lève.
    /// Appel imbriqué : réutilise la transaction englobante. Les <see cref="CommitAsync"/> internes (et l'historique) y participent.
    /// </summary>
    Task ExecuterDansTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
}
