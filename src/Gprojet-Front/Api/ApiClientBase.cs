using System.Net.Http.Json;
using System.Text.Json;
using Gprojet.Backend.Contracts.Commun;

namespace Gprojet.Front.Api;

/// <summary>Seule classe du front qui touche HttpClient : JSON (Web), ProblemDetails -> ApiException.</summary>
public abstract class ApiClientBase(HttpClient http)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    protected Task<T?> GetAsync<T>(string url, CancellationToken ct = default) =>
        EnvoyerAsync<T>(new HttpRequestMessage(HttpMethod.Get, url), ct);

    protected Task<T?> PostAsync<T>(string url, object? corps, CancellationToken ct = default) =>
        EnvoyerAsync<T>(Avec(HttpMethod.Post, url, corps), ct);

    protected Task<T?> PutAsync<T>(string url, object? corps, CancellationToken ct = default) =>
        EnvoyerAsync<T>(Avec(HttpMethod.Put, url, corps), ct);

    protected Task DeleteAsync(string url, CancellationToken ct = default) =>
        EnvoyerAsync<object>(new HttpRequestMessage(HttpMethod.Delete, url), ct);

    private static HttpRequestMessage Avec(HttpMethod methode, string url, object? corps) =>
        new(methode, url) { Content = JsonContent.Create(corps, options: Json) };

    private async Task<T?> EnvoyerAsync<T>(HttpRequestMessage requete, CancellationToken ct)
    {
        if (requete.Method != HttpMethod.Get)
        {
            requete.Headers.Add(ProtectionCsrf.EnTete, ProtectionCsrf.Valeur); // anti-CSRF du BFF, sur toute méthode mutante
        }

        using var reponse = await http.SendAsync(requete, ct);
        if (!reponse.IsSuccessStatusCode)
        {
            throw await LireErreurAsync(reponse, ct);
        }

        if (reponse.Content.Headers.ContentLength is 0 || reponse.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return default;
        }

        return await reponse.Content.ReadFromJsonAsync<T>(Json, ct);
    }

    private static async Task<ApiException> LireErreurAsync(HttpResponseMessage reponse, CancellationToken ct)
    {
        var statut = (int)reponse.StatusCode;
        try
        {
            var p = await reponse.Content.ReadFromJsonAsync<Probleme>(Json, ct);
            return p is null ? new ApiException(statut) : new ApiException(statut, p.Title, p.Detail, p.Type, p.Errors);
        }
        catch (JsonException) { return new ApiException(statut); } // corps absent ou non problem+json
        catch (NotSupportedException) { return new ApiException(statut); } // type de contenu non JSON
    }

    private sealed record Probleme(string? Type, string? Title, string? Detail, Dictionary<string, string[]>? Errors);
}
