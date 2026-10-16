using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Gprojet.Backend.Infrastructure.Authentification;

/// <summary>
/// Schéma « Dev » (Development / Test uniquement) : l'agent est désigné par l'en-tête <c>X-Dev-Matricule</c>, sans secret.
/// Remplacé par Keycloak en P18. L'identité vient de <see cref="IChargeurIdentite"/>, partagé avec le login BFF de développement.
/// </summary>
public sealed class DevAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IChargeurIdentite chargeur)
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

        var principal = await chargeur.ChargerAsync(valeur.ToString().Trim(), Context.RequestAborted);
        return principal is null
            ? AuthenticateResult.Fail("Matricule inconnu ou agent inactif.")
            : AuthenticateResult.Success(new AuthenticationTicket(principal, Schema));
    }
}
