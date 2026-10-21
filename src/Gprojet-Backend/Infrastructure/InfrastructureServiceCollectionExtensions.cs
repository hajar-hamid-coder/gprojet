using Gprojet.Backend.Common;
using Gprojet.Backend.Infrastructure.Authentification;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Gprojet.Backend.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>Environnements où le schéma d'authentification « Dev » (en-tête X-Dev-Matricule) et le login BFF de développement existent.</summary>
    public static bool AuthentificationDevAutorisee(IHostEnvironment env) => env.IsDevelopment() || env.IsEnvironment("Test");

    public static IServiceCollection AddGpInfrastructure(this IServiceCollection services, IHostEnvironment env)
    {
        services.AddSingleton<IHorloge, HorlogeSysteme>();
        services.AddScoped<ContexteRequete>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, UtilisateurCourant>();

        // shortcut: authentification de développement seule ; Keycloak (OIDC) la remplace en P18. Ailleurs : « Refus » = 401 systématique.
        // Dev/Test : schéma « Gp » = en-tête X-Dev-Matricule s'il est présent (tests, outils), sinon cookie de session (login BFF).
        if (AuthentificationDevAutorisee(env))
        {
            services.AddScoped<IChargeurIdentite, ChargeurIdentiteDev>();
            services.AddAuthentication(CookieSession.SchemaCombine)
                .AddPolicyScheme(CookieSession.SchemaCombine, null, o => o.ForwardDefaultSelector =
                    ctx => ctx.Request.Headers.ContainsKey(DevAuthHandler.EnTete) ? DevAuthHandler.Schema : CookieAuthenticationDefaults.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, DevAuthHandler>(DevAuthHandler.Schema, null)
                .AddCookie(o =>
                {
                    o.Cookie.Name = CookieSession.Nom;
                    o.Cookie.HttpOnly = true;
                    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    o.Cookie.SameSite = SameSiteMode.Strict;
                    o.ExpireTimeSpan = TimeSpan.FromHours(8);
                    o.SlidingExpiration = true;
                    // SPA : pas de redirection vers une page de login, l'API répond 401 / 403.
                    o.Events.OnRedirectToLogin = c => { c.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
                    o.Events.OnRedirectToAccessDenied = c => { c.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
                });
        }
        else
        {
            services.AddAuthentication(SchemaRefus.Schema).AddScheme<AuthenticationSchemeOptions, SchemaRefus>(SchemaRefus.Schema, null);
        }

        services.AddAuthorization();
        return services;
    }
}
