using System.Net.Http.Json;
using Gprojet.Backend.Contracts.Commun;

namespace Gprojet.Front.Api;

public sealed class SanteApiClient(HttpClient http)
{
    public Task<SanteDto?> LireAsync(CancellationToken ct = default) => http.GetFromJsonAsync<SanteDto>("/api/health", ct);
}
