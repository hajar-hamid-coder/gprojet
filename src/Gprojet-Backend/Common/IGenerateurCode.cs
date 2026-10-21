namespace Gprojet.Backend.Common;

/// <summary>Produit le prochain code lisible, unique et sans trou de lecture, au format PREFIXE-AAAA-NNNN (ex. PRJ-2026-0001).</summary>
public interface IGenerateurCode
{
    Task<string> ProchainCodeAsync(string prefixe, CancellationToken ct = default);
}
