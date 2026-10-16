using Gprojet.Backend.Common;
using Microsoft.EntityFrameworkCore;

namespace Gprojet.Backend.Repositories;

/// <summary>Implémentation EF Core de <see cref="IUnitOfWork"/> (portée DI : un <see cref="GpDbContext"/> par requête).</summary>
public sealed class UnitOfWork(GpDbContext db) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        try
        {
            return await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Pas de détail interne (SQL, entités) dans le message : il remonte jusqu'au ProblemDetails 409.
            throw new ConflitException("Les données ont été modifiées ou supprimées par un autre utilisateur. Rechargez puis recommencez.");
        }
    }

    public async Task ExecuterDansTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
    {
        if (db.Database.CurrentTransaction is not null)
        {
            await action(ct);
            return;
        }

        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        await action(ct); // exception : Dispose sans Commit = rollback
        await transaction.CommitAsync(ct);
    }
}
