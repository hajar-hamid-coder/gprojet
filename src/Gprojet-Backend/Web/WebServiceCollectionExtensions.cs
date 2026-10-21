using Gprojet.Backend.Web.Erreurs;

namespace Gprojet.Backend.Web;

public static class WebServiceCollectionExtensions
{
    /// <summary>Contrôleurs, erreurs en ProblemDetails et OpenAPI natif.</summary>
    public static IServiceCollection AddGpWeb(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddProblemDetails();
        services.AddExceptionHandler<GestionnaireExceptions>();
        services.AddOpenApi();
        return services;
    }
}
