using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Gprojet.Front.Auth;

// shortcut: toujours anonyme, le BFF (/bff/user) n'est pas encore branché ; remplacer par le fournisseur BFF à la tâche d'authentification front.
public sealed class AuthentificationProvisoire : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
}
