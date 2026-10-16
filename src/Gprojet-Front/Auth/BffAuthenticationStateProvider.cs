using System.Security.Claims;
using Gprojet.Front.Api;
using Microsoft.AspNetCore.Components.Authorization;

namespace Gprojet.Front.Auth;

/// <summary>
/// État d'authentification lu une fois via <c>/bff/user</c> puis mis en cache. Login et logout rechargent la page entière
/// (formulaire / navigation complète) : le cache repart alors à neuf, sans invalidation à gérer.
/// </summary>
public sealed class BffAuthenticationStateProvider(BffApiClient client) : AuthenticationStateProvider
{
    private Task<AuthenticationState>? _etat;

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _etat ??= LireAsync();

    private async Task<AuthenticationState> LireAsync()
    {
        try
        {
            if (await client.UtilisateurAsync() is { } u)
            {
                var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, u.Id.ToString()), new(ClaimTypes.Name, u.Matricule) };
                claims.AddRange(u.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "bff")));
            }
        }
        catch (Exception e) when (e is ApiException or HttpRequestException)
        {
            // Backend injoignable ou en erreur : on reste anonyme.
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}
