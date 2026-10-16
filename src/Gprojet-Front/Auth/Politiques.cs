using Gprojet.Backend.Contracts.Commun;
using Microsoft.AspNetCore.Authorization;

namespace Gprojet.Front.Auth;

/// <summary>Politiques d'affichage du front (le serveur reste l'autorité : masquer un lien ne protège rien).</summary>
public static class Politiques
{
    public static void Enregistrer(AuthorizationOptions options)
    {
        options.AddPolicy(RolesApplication.PolitiquePortefeuilleDirection, p => p.RequireRole(RolesApplication.Directeur));
        options.AddPolicy(RolesApplication.PolitiqueAdministration,
            p => p.RequireRole(RolesApplication.Directeur, RolesApplication.ResponsableEntite));
    }
}
