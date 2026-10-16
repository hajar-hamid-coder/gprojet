using Gprojet.Backend.Repositories.Audit;
using Gprojet.Backend.Repositories.Organisation;
using Microsoft.EntityFrameworkCore;

namespace Gprojet.Backend.Repositories;

public static class RepositoriesServiceCollectionExtensions
{
    /// <summary>Enregistre le DbContext (<c>ConnectionStrings:Gp</c>), l'audit, l'unité de travail, les repositories et le contrôle de santé de la base.</summary>
    public static IServiceCollection AddGpRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        // La valeur n'est jamais citée dans le message : c'est un secret.
        var chaine = configuration.GetConnectionString("Gp")
                     ?? throw new InvalidOperationException("Chaîne de connexion 'ConnectionStrings:Gp' absente (variable d'environnement ConnectionStrings__Gp).");

        services.AddScoped<AuditInterceptor>();
        services.AddDbContext<GpDbContext>((sp, options) => options
            .UseNpgsql(chaine)
            .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICompetenceRepository, CompetenceRepository>();
        services.AddHealthChecks().AddDbContextCheck<GpDbContext>("base", tags: ["ready"]);
        return services;
    }
}
