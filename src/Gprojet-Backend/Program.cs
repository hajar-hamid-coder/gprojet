using Gprojet.Backend.Contracts.Commun;
using Gprojet.Backend.Infrastructure;
using Gprojet.Backend.Repositories;
using Gprojet.Backend.Web;
using Gprojet.Backend.Web.Auth;
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
app.UseStatusCodePages(); // 404/401 sans corps -> ProblemDetails
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseSerilogRequestLogging();

app.UseProtectionCsrf();
app.UseAuthentication();
app.UseAuthorization();

// Tout contrôleur exige un utilisateur authentifié (/health*, /openapi, /scalar restent anonymes).
app.MapControllers().RequireAuthorization();
app.MapBff(InfrastructureServiceCollectionExtensions.AuthentificationDevAutorisee(app.Environment));
app.MapOpenApi();
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(o => o.DisableDefaultFonts()); // aucune police chargée depuis Internet
}

app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = c => c.Tags.Contains("ready") });

app.MapGet("/api/health", () => new SanteDto("ok")).AllowAnonymous();

// Le front prend tout sauf l'API et les points techniques : une route inconnue de ceux-ci reste un 404 JSON.
app.MapFallbackToFile("/", "index.html"); // la racine : un catch-all ne reconnaît pas le chemin vide
app.MapFallbackToFile("{**chemin:regex(^(?!api(/|$)|bff(/|$)|health|openapi|scalar).*$)}", "index.html");

app.Run();

// Visible des tests d'intégration (WebApplicationFactory).
public partial class Program;
