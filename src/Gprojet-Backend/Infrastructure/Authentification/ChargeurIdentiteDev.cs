using System.Security.Claims;
using Gprojet.Backend.Common;
using Gprojet.Backend.Contracts.Commun;
using Gprojet.Backend.Entities.Organisation;
using Gprojet.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gprojet.Backend.Infrastructure.Authentification;

/// <summary>
/// Identité de développement : agent actif désigné par son matricule, rôles déduits de l'organigramme par deux requêtes d'existence.
/// Partagé par le schéma « Dev » (en-tête) et par le login BFF de développement. Remplacé par Keycloak en P18.
/// </summary>
public sealed class ChargeurIdentiteDev(GpDbContext db, IHorloge horloge) : IChargeurIdentite
{
    public async Task<ClaimsPrincipal?> ChargerAsync(string matricule, CancellationToken ct)
    {
        var agent = await db.Set<Agent>().AsNoTracking()
            .Where(a => a.Matricule == matricule && a.Actif)
            .Select(a => new { a.Id, a.Matricule, a.EntiteId })
            .FirstOrDefaultAsync(ct);
        if (agent is null)
        {
            return null;
        }

        var aujourdhui = horloge.Aujourdhui;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, agent.Id.ToString()),
            new(ClaimTypes.Name, agent.Matricule),
            new(DevAuthHandler.ClaimEntite, agent.EntiteId.ToString()),
        };
        if (await db.Set<EntiteOrganisationnelle>().AnyAsync(e => e.ResponsableId == agent.Id && e.Actif, ct))
        {
            claims.Add(new Claim(ClaimTypes.Role, RolesApplication.ResponsableEntite));
        }

        if (await db.Set<Delegation>().AnyAsync(d => d.DelegataireId == agent.Id && d.DateDebut <= aujourdhui && d.DateFin >= aujourdhui, ct))
        {
            claims.Add(new Claim(ClaimTypes.Role, RolesApplication.Delegataire));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, DevAuthHandler.Schema));
    }
}
