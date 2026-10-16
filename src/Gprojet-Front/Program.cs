using Gprojet.Front;
using Gprojet.Front.Api;
using Gprojet.Front.Auth;
using Gprojet.Front.Services;
using Gprojet.Front.State;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Même origine que le backend (BFF) : adresses relatives, ni proxy ni CORS.
builder.Services.AddHttpClient<SanteApiClient>(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddHttpClient<BffApiClient>(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddAuthorizationCore(Politiques.Enregistrer);
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();

builder.Services.AddScoped<UtilisateurCourantState>();
builder.Services.AddScoped<PerimetreState>();
builder.Services.AddScoped<NotificationUiService>();
builder.Services.AddScoped<ChargementService>();
builder.Services.AddScoped<GestionnaireErreursUi>();

await builder.Build().RunAsync();
