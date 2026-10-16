using Gprojet.Backend.Common;
using Gprojet.Backend.Contracts.Commun;
using Gprojet.Backend.Infrastructure.Authentification;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Gprojet.Backend.Web.Auth;

public static class BffEndpoints
{
    public const string CheminLogin = "/bff/login";

    /// <summary>/bff/user et /bff/logout partout ; le login de développement n'est mappé qu'en Development / Test.</summary>
    public static IEndpointRouteBuilder MapBff(this IEndpointRouteBuilder app, bool loginDev)
    {
        app.MapGet("/bff/user", (ICurrentUser u) => new UtilisateurDto(u.Id, u.Matricule, u.EntiteId, [.. u.Roles]))
            .RequireAuthorization();

        app.MapPost("/bff/logout", async (HttpContext ctx) =>
        {
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.NoContent();
        }).RequireAuthorization();

        if (loginDev)
        {
            // Sert de sonde au front : 204 si le login de développement existe (sinon 404).
            app.MapGet(CheminLogin, () => Results.NoContent()).AllowAnonymous();

            // Formulaire classique : le cookie est posé par la réponse de navigation, redirection fixe (pas d'open redirect).
            app.MapPost(CheminLogin, async (HttpContext ctx, [FromForm] string? matricule, IChargeurIdentite chargeur) =>
            {
                var principal = string.IsNullOrWhiteSpace(matricule) ? null : await chargeur.ChargerAsync(matricule.Trim(), ctx.RequestAborted);
                if (principal is null)
                {
                    return Results.Redirect("/connexion?erreur=1");
                }

                await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return Results.Redirect("/");
            }).AllowAnonymous().DisableAntiforgery();
        }

        return app;
    }
}
