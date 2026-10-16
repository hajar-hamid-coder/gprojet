using System.Security.Claims;
using Gprojet.Backend.Common;

namespace Gprojet.Backend.Infrastructure.Authentification;

/// <summary>
/// Utilisateur de la requête, lu dans les claims. Sans utilisateur authentifié (hors requête HTTP : migrations, tâches de fond ;
/// ou endpoint anonyme), on retombe sur l'agent « système » sans entité ni rôle : le moins de droits possible.
/// </summary>
public sealed class UtilisateurCourant(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User is { Identity.IsAuthenticated: true } p ? p : null;

    public Guid Id => Guid.TryParse(Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

    public string Matricule => Principal?.FindFirstValue(ClaimTypes.Name) ?? "SYSTEME";

    public Guid EntiteId => Guid.TryParse(Principal?.FindFirstValue(DevAuthHandler.ClaimEntite), out var id) ? id : Guid.Empty;

    public IReadOnlyCollection<string> Roles => Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray() ?? [];
}
