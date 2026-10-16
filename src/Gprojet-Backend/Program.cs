using Gprojet.Backend.Infrastructure;
using Gprojet.Backend.Repositories;
using Gprojet.Backend.Web;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console(new RenderedCompactJsonFormatter()));

builder.Services
    .AddGpInfrastructure(builder.Environment)
    .AddGpRepositories(builder.Configuration)
    .AddGpWeb();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

// Tout contrôleur exige un utilisateur authentifié (/health*, /openapi, /scalar restent anonymes).
app.MapControllers().RequireAuthorization();
app.MapOpenApi();
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(o => o.DisableDefaultFonts()); // aucune police chargée depuis Internet
}

app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = c => c.Tags.Contains("ready") });

app.Run();

// Visible des tests d'intégration (WebApplicationFactory).
public partial class Program;
