using Gprojet.Backend.Contracts.Commun;

namespace Gprojet.Front.Api;

public sealed class SanteApiClient(HttpClient http) : ApiClientBase(http)
{
    public Task<SanteDto?> LireAsync(CancellationToken ct = default) => GetAsync<SanteDto>("api/health", ct);
}
