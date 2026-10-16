using Gprojet.Backend.Common;

namespace Gprojet.Backend.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddGpInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IHorloge, HorlogeSysteme>();
        services.AddScoped<ContexteRequete>();
        services.AddScoped<ICurrentUser, UtilisateurProvisoire>();
        return services;
    }
}
