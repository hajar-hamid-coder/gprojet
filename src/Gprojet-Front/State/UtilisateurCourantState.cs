using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Gprojet.Front.State;

/// <summary>Utilisateur courant dérivé des claims de l'<see cref="AuthenticationStateProvider"/> (un seul appel /bff/user).</summary>
public sealed class UtilisateurCourantState : IDisposable
{
    private readonly AuthenticationStateProvider _fournisseur;

    public UtilisateurCourantState(AuthenticationStateProvider fournisseur)
    {
        _fournisseur = fournisseur;
        _fournisseur.AuthenticationStateChanged += SurChangement;
    }

    public event Action? Changed;

    public async Task<UtilisateurCourant> ObtenirAsync()
    {
        var u = (await _fournisseur.GetAuthenticationStateAsync()).User;
        Guid? id = Guid.TryParse(u.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var g) ? g : null;
        return new UtilisateurCourant(u.Identity?.IsAuthenticated == true, id, u.Identity?.Name,
            [.. u.FindAll(ClaimTypes.Role).Select(c => c.Value)]);
    }

    private void SurChangement(Task<AuthenticationState> _) => Changed?.Invoke();

    public void Dispose() => _fournisseur.AuthenticationStateChanged -= SurChangement;
}

public sealed record UtilisateurCourant(bool Authentifie, Guid? Id, string? Matricule, IReadOnlyList<string> Roles);
