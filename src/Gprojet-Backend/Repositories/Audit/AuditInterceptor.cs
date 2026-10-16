using System.Globalization;
using Gprojet.Backend.Common;
using Gprojet.Backend.Entities;
using Gprojet.Backend.Entities.Commun;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gprojet.Backend.Repositories.Audit;

/// <summary>
/// Audit, suppression logique et historique chaîné (<c>commun.historique</c>) à chaque <c>SaveChangesAsync</c>.
/// À enregistrer en portée DI (une instance par DbContext : elle retient la transaction qu'elle ouvre).
/// Choix : <c>type_cible</c> = nom de table sans schéma (varchar(30)) ; CREATION = une ligne par entité (champ NULL) ;
/// MODIFICATION = une ligne par champ réellement changé ; SUPPRESSION / RESTAURATION = une ligne (champ NULL).
/// Concurrence : l'historique est écrit dans la même transaction que les données, sous un verrou consultatif global
/// (<c>pg_advisory_xact_lock</c>) qui sérialise la lecture de la dernière empreinte et l'insertion.
/// </summary>
public sealed class AuditInterceptor(ICurrentUser utilisateur, IHorloge horloge, ContexteRequete requete) : SaveChangesInterceptor
{
    private const long VerrouHistorique = 0x4850_4348_4953_5400; // arbitraire, propre à l'historique

    private static readonly HashSet<string> NonHistorises =
    [
        nameof(EntiteAuditable.Id), nameof(EntiteAuditable.CreeLe), nameof(EntiteAuditable.CreePar), nameof(EntiteAuditable.ModifieLe),
        nameof(EntiteAuditable.ModifiePar), nameof(EntiteAuditable.EstSupprime), nameof(EntiteAuditable.SupprimeLe),
        nameof(EntiteAuditable.SupprimePar), nameof(EntiteAuditable.VersionLigne),
    ];

    private IDbContextTransaction? _transaction;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) =>
        throw new NotSupportedException("L'audit exige SaveChangesAsync (pas de contournement synchrone).");

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var db = eventData.Context!;
        var maintenant = horloge.Maintenant;
        var lignes = new List<Historique>();

        db.ChangeTracker.DetectChanges();
        foreach (var e in db.ChangeTracker.Entries<EntiteAuditable>().ToList())
        {
            var table = e.Metadata.GetTableName()!;
            if (e.State == EntityState.Deleted)
            {
                e.State = EntityState.Modified; // suppression logique
                e.Entity.EstSupprime = true;
            }

            if (e.State == EntityState.Added)
            {
                e.Entity.CreeLe = maintenant;
                e.Entity.CreePar = utilisateur.Id;
                lignes.Add(Ligne(table, e.Entity.Id, ActionHistorique.Creation, null, null, null));
            }
            else if (e.State == EntityState.Modified)
            {
                var estSupprime = e.Property(x => x.EstSupprime);
                var bascule = estSupprime.IsModified && estSupprime.OriginalValue != estSupprime.CurrentValue;
                e.Entity.ModifieLe = maintenant;
                e.Entity.ModifiePar = utilisateur.Id;
                if (bascule && e.Entity.EstSupprime)
                {
                    e.Entity.SupprimeLe = maintenant;
                    e.Entity.SupprimePar = utilisateur.Id;
                    lignes.Add(Ligne(table, e.Entity.Id, ActionHistorique.Suppression, null, null, null));
                }
                else if (bascule)
                {
                    lignes.Add(Ligne(table, e.Entity.Id, ActionHistorique.Restauration, null, null, null));
                }

                lignes.AddRange(Modifications(e, table));
            }
        }

        if (lignes.Count > 0)
        {
            if (db.Database.CurrentTransaction is null)
            {
                _transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            }

            await db.Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock({VerrouHistorique})", cancellationToken);
            var precedente = await db.Set<Historique>().AsNoTracking().OrderByDescending(h => h.Id)
                .Select(h => h.Empreinte).FirstOrDefaultAsync(cancellationToken);
            foreach (var h in lignes)
            {
                h.EmpreintePrecedente = precedente;
                h.Empreinte = precedente = EmpreinteHistorique.Calculer(precedente, h);
            }

            db.Set<Historique>().AddRange(lignes);
        }

        return result;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (_transaction is { } t)
        {
            _transaction = null;
            await using (t)
            {
                await t.CommitAsync(cancellationToken);
            }
        }

        return result;
    }

    public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default) =>
        await AnnulerAsync();

    public override async Task SaveChangesCanceledAsync(DbContextEventData eventData, CancellationToken cancellationToken = default) =>
        await AnnulerAsync();

    private async Task AnnulerAsync()
    {
        if (_transaction is { } t)
        {
            _transaction = null;
            await t.DisposeAsync(); // rollback implicite
        }
    }

    private Historique Ligne(string table, Guid cibleId, ActionHistorique action, string? champ, string? ancienne, string? nouvelle) => new()
    {
        TypeCible = table, CibleId = cibleId, Action = action, Champ = champ, AncienneValeur = ancienne, NouvelleValeur = nouvelle,
        AgentId = utilisateur.Id, DateAction = EmpreinteHistorique.Microseconde(horloge.Maintenant), CorrelationId = requete.CorrelationId,
    };

    private IEnumerable<Historique> Modifications(EntityEntry<EntiteAuditable> e, string table)
    {
        foreach (var p in e.Properties.Where(p => p.IsModified && !NonHistorises.Contains(p.Metadata.Name)))
        {
            var avant = Texte(p.OriginalValue);
            var apres = Texte(p.CurrentValue);
            if (avant != apres)
            {
                yield return Ligne(table, e.Entity.Id, ActionHistorique.Modification, p.Metadata.GetColumnName(), avant, apres);
            }
        }
    }

    private static string? Texte(object? v) => v switch
    {
        null => null,
        Enum en => ValeurEnumSql.VersSql(en),
        bool b => b ? "true" : "false",
        DateTimeOffset d => d.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFF'Z'", CultureInfo.InvariantCulture),
        IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
        _ => v.ToString(),
    };
}
