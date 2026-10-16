using System.Security.Claims;

namespace Gprojet.Backend.Infrastructure.Authentification;

/// <summary>Construit l'identité (claims) d'un agent à partir de son matricule ; null si inconnu, inactif ou supprimé.</summary>
public interface IChargeurIdentite
{
    Task<ClaimsPrincipal?> ChargerAsync(string matricule, CancellationToken ct);
}
