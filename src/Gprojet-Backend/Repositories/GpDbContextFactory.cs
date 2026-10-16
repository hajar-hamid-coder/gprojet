using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gprojet.Backend.Repositories;

/// <summary>Fabrique pour l'outillage <c>dotnet ef</c> : chaîne lue dans la variable d'environnement <c>ConnectionStrings__Gp</c>.</summary>
public sealed class GpDbContextFactory : IDesignTimeDbContextFactory<GpDbContext>
{
    public GpDbContext CreateDbContext(string[] args)
    {
        var chaine = Environment.GetEnvironmentVariable("ConnectionStrings__Gp")
                     ?? throw new InvalidOperationException("Variable d'environnement ConnectionStrings__Gp absente.");

        return new GpDbContext(new DbContextOptionsBuilder<GpDbContext>().UseNpgsql(chaine).Options);
    }
}
