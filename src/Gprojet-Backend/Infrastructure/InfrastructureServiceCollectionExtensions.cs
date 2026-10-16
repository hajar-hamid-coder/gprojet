using Gprojet.Backend.Common;
using Gprojet.Backend.Infrastructure.Authentification;
using Microsoft.AspNetCore.Authentication;

namespace Gprojet.Backend.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>Environnements où le schéma d'authentification « Dev » (en-tête X-Dev-Matricule) existe.</summary>
    public static bool AuthentificationDevAutorisee(IHostEnvironment env) => env.IsDevelopment() || env.IsEnvironment("Test");

    public static IServiceCollection AddGpInfrastructure(this IServiceCollection services, IHostEnvironment env)
    {
        services.AddSingleton<IHorloge, HorlogeSysteme>();
        services.AddScoped<ContexteRequete>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, UtilisateurCourant>();

        // shortcut: authentification de développement seule ; Keycloak (OIDC) la remplace en P18. Ailleurs : « Refus » = 401 systématique.
        var auth = services.AddAuthentication(AuthentificationDevAutorisee(env) ? DevAuthHandler.Schema : SchemaRefus.Schema);
        if (AuthentificationDevAutorisee(env))
        {
            auth.AddScheme<AuthenticationSchemeOptions, DevAuthHandler>(DevAuthHandler.Schema, null);
        }
        else
        {
            auth.AddScheme<AuthenticationSchemeOptions, SchemaRefus>(SchemaRefus.Schema, null);
        }

        services.AddAuthorization();
        return services;
    }
}
