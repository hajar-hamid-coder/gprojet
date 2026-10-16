using System.Security.Claims;
using System.Text.Encodings.Web;
using Gprojet.Backend.Common;
using Gprojet.Backend.Contracts.Commun;
using Gprojet.Backend.Entities.Organisation;
using Gprojet.Backend.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gprojet.Backend.Infrastructure.Authentification;

/// <summary>
/// Schéma « Dev » (Development / Test uniquement) : l'agent est désigné par l'en-tête <c>X-Dev-Matricule</c>, sans secret.
/// Remplacé par Keycloak en P18. Les rôles sont déduits de l'organigramme par deux requêtes d'existence (pas de graphe chargé).
/// </summary>
public sealed class DevAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, GpDbContext db, IHorloge horloge)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string Schema = "Dev";
    public const string EnTete = "X-Dev-Matricule";
    public const string ClaimEntite = "entite_id";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(EnTete, out var valeur) || string.IsNullOrWhiteSpace(valeur))
        {
            return AuthenticateResult.NoResult();
        }

        var matricule = valeur.ToString().Trim();
        var ct = Context.RequestAborted;
        var agent = await db.Set<Agent>().AsNoTracking()
            .Where(a => a.Matricule == matricule && a.Actif)
            .Select(a => new { a.Id, a.Matricule, a.EntiteId })
            .FirstOrDefaultAsync(ct);
        if (agent is null)
        {
            return AuthenticateResult.Fail("Matricule inconnu ou agent inactif.");
        }

        var aujourdhui = horloge.Aujourdhui;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, agent.Id.ToString()),
            new(ClaimTypes.Name, agent.Matricule),
            new(ClaimEntite, agent.EntiteId.ToString()),
        };
        if (await db.Set<EntiteOrganisationnelle>().AnyAsync(e => e.ResponsableId == agent.Id && e.Actif, ct))
        {
            claims.Add(new Claim(ClaimTypes.Role, RolesApplication.ResponsableEntite));
        }

        if (await db.Set<Delegation>().AnyAsync(d => d.DelegataireId == agent.Id && d.DateDebut <= aujourdhui && d.DateFin >= aujourdhui, ct))
        {
            claims.Add(new Claim(ClaimTypes.Role, RolesApplication.Delegataire));
        }

        var identite = new ClaimsIdentity(claims, Schema);
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identite), Schema));
    }
}
