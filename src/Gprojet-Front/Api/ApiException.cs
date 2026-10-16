namespace Gprojet.Front.Api;

/// <summary>Erreur renvoyée par l'API (ProblemDetails) ou, à défaut, simple statut HTTP.</summary>
public sealed class ApiException(int statut, string? titre = null, string? detail = null, string? type = null,
    IReadOnlyDictionary<string, string[]>? erreurs = null)
    : Exception(detail ?? titre ?? $"Erreur HTTP {statut}")
{
    public int Statut { get; } = statut;
    public string? Titre { get; } = titre;
    public string? Detail { get; } = detail;
    public string? Type { get; } = type;
    public IReadOnlyDictionary<string, string[]> Erreurs { get; } = erreurs ?? new Dictionary<string, string[]>();
}
