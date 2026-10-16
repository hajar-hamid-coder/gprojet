using Gprojet.Front;
using Gprojet.Front.Api;
using Gprojet.Front.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Même origine que le backend (BFF) : adresses relatives, ni proxy ni CORS.
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<SanteApiClient>();

builder.Services.AddAuthorizationCore(Politiques.Enregistrer);
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AuthentificationProvisoire>();

await builder.Build().RunAsync();
