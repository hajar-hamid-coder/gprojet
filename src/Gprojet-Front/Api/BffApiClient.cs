using Gprojet.Backend.Contracts.Commun;

namespace Gprojet.Front.Api;

/// <summary>Session côté BFF : agent connecté, déconnexion, présence du login de développement.</summary>
public sealed class BffApiClient(HttpClient http) : ApiClientBase(http)
{
    /// <summary>Agent connecté, ou null si aucune session (401).</summary>
    public async Task<UtilisateurDto?> UtilisateurAsync(CancellationToken ct = default)
    {
        try
        {
            return await GetAsync<UtilisateurDto>("bff/user", ct);
        }
        catch (ApiException e) when (e.Statut == 401)
        {
            return null;
        }
    }

    public Task ChangerLangueAsync(string culture, CancellationToken ct = default) => PostAsync<object>("bff/langue", new LangueDto(culture), ct);

    public Task DeconnecterAsync(CancellationToken ct = default) => PostAsync<object>("bff/logout", null, ct);

    /// <summary>Le backend n'expose le login de développement qu'en Development / Test (404 sinon).</summary>
    public async Task<bool> LoginDevDisponibleAsync(CancellationToken ct = default)
    {
        try
        {
            await GetAsync<object>("bff/login", ct);
            return true;
        }
        catch (ApiException e) when (e.Statut == 404)
        {
            return false;
        }
    }
}
