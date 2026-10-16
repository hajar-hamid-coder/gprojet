using Gprojet.Backend.Contracts.Commun;
using Gprojet.Backend.Infrastructure.Authentification;

namespace Gprojet.Backend.Web.Auth;

/// <summary>
/// Anti-CSRF des BFF usuels : en-tête personnalisé obligatoire (<c>X-CSRF: 1</c>) sur POST/PUT/PATCH/DELETE de /api et /bff
/// dès que le cookie de session est présent. Un site tiers ne peut pas ajouter d'en-tête personnalisé sans préflight CORS
/// (aucun CORS n'est ouvert), et le cookie est SameSite=Strict. Exemptions : /bff/login (aucune session à abuser) et les appels
/// sans cookie (en-tête X-Dev-Matricule, Dev/Test seulement : un navigateur ne l'ajoute jamais tout seul, donc non forgeable en CSRF).
/// </summary>
public static class ProtectionCsrfMiddleware
{
    public static IApplicationBuilder UseProtectionCsrf(this IApplicationBuilder app) => app.Use(async (ctx, next) =>
    {
        var r = ctx.Request;
        var mutante = !(HttpMethods.IsGet(r.Method) || HttpMethods.IsHead(r.Method) || HttpMethods.IsOptions(r.Method) || HttpMethods.IsTrace(r.Method));
        var concerne = r.Path.StartsWithSegments("/api") || r.Path.StartsWithSegments("/bff");
        if (mutante && concerne && !r.Path.StartsWithSegments(BffEndpoints.CheminLogin)
            && r.Cookies.ContainsKey(CookieSession.Nom) && r.Headers[ProtectionCsrf.EnTete] != ProtectionCsrf.Valeur)
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await next();
    });
}
