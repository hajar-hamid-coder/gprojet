using System.Linq.Expressions;
using Gprojet.Backend.Common;
using Gprojet.Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gprojet.Backend.Repositories;

/// <summary>
/// Socle commun des repositories d'agrégat (interne : jamais exposé ni injecté, seules les interfaces d'agrégat le sont).
/// La suppression est logique : <see cref="Supprimer"/> appelle <c>Remove</c>, que l'<c>AuditInterceptor</c> convertit.
/// </summary>
internal abstract class RepositoryBase<T>(GpDbContext db) where T : EntiteAuditable
{
    protected GpDbContext Db { get; } = db;

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Db.Set<T>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public void Ajouter(T entite) => Db.Set<T>().Add(entite);

    public void Supprimer(T entite) => Db.Set<T>().Remove(entite);

    /// <summary>Page triée par Id (unique) : tri stable, donc pagination déterministe.</summary>
    protected async Task<PageResultat<T>> PageAsync(PageRequete page, Expression<Func<T, bool>>? filtre = null, CancellationToken ct = default)
    {
        var requete = filtre is null ? Db.Set<T>().AsNoTracking() : Db.Set<T>().AsNoTracking().Where(filtre);
        var total = await requete.CountAsync(ct);
        var elements = await requete.OrderBy(e => e.Id).Skip(page.Decalage).Take(page.Taille).ToListAsync(ct);
        return new PageResultat<T>(elements, total, page.Page, page.Taille);
    }
}
