using FluentValidation;
using Gprojet.Backend.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Gprojet.Backend.Web.Erreurs;

/// <summary>
/// Traduit les exceptions en <c>ProblemDetails</c> : validation 400, accès refusé 403, introuvable 404, conflit 409, règle métier 422
/// (cohérent avec les commentaires de Common) ; tout le reste 500 (déjà journalisé par le middleware d'exceptions). Le détail d'une erreur inattendue n'est exposé qu'en Development.
/// </summary>
public sealed class GestionnaireExceptions(IProblemDetailsService problemes, IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statut, titre, detail) = exception switch
        {
            ValidationException v => (400, "Requête invalide.", string.Join(" ", v.Errors.Select(e => e.ErrorMessage).Distinct())),
            AccesRefuseException e => (403, "Accès refusé.", e.Message),
            IntrouvableException e => (404, "Ressource introuvable.", e.Message),
            ConflitException e => (409, "Conflit.", e.Message),
            RegleMetierException e => (422, "Règle métier non respectée.", e.Message),
            _ => (500, "Erreur interne.", env.IsDevelopment() ? exception.ToString() : null),
        };

        httpContext.Response.StatusCode = statut;
        return await problemes.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Status = statut, Title = titre, Detail = detail },
        });
    }
}
