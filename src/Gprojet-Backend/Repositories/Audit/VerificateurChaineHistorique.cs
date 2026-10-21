using Gprojet.Backend.Entities.Commun;
using Microsoft.EntityFrameworkCore;

namespace Gprojet.Backend.Repositories.Audit;

/// <summary>Relit <c>commun.historique</c> dans l'ordre des id, recalcule chaque empreinte et renvoie la première ligne invalide (ou null).</summary>
public sealed class VerificateurChaineHistorique(GpDbContext db)
{
    public async Task<Historique?> PremiereLigneInvalideAsync(CancellationToken ct = default)
    {
        string? precedente = null;
        await foreach (var h in db.Set<Historique>().AsNoTracking().OrderBy(x => x.Id).AsAsyncEnumerable().WithCancellation(ct))
        {
            if (h.EmpreintePrecedente != precedente || h.Empreinte != EmpreinteHistorique.Calculer(precedente, h))
            {
                return h;
            }

            precedente = h.Empreinte;
        }

        return null;
    }
}
