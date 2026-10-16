using Gprojet.Backend.Common;

namespace Gprojet.Backend.Infrastructure;

/// <summary>Agent « système » sans droits, utilisé tant que l'authentification Keycloak n'est pas branchée.</summary>
// shortcut: aucune authentification, tout appel est attribué à l'agent système ; à remplacer par l'utilisateur issu du cookie/jeton en P18 (Keycloak).
public sealed class UtilisateurProvisoire : ICurrentUser
{
    public Guid Id => Guid.Empty;

    public string Matricule => "SYSTEME";

    public Guid EntiteId => Guid.Empty;

    public IReadOnlyCollection<string> Roles => [];
}
