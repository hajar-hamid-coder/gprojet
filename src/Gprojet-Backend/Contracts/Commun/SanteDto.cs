namespace Gprojet.Backend.Contracts.Commun;

/// <summary>Réponse de GET /api/health : le front s'en sert pour savoir que l'API répond.</summary>
public sealed record SanteDto(string Statut);
