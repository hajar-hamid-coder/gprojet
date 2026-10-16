namespace Gprojet.Backend.Common;

/// <summary>Identifiant de corrélation d'une requête (une instance par portée DI) : regroupe les lignes d'historique d'une même requête.</summary>
public sealed class ContexteRequete
{
    public Guid CorrelationId { get; } = Guid.CreateVersion7();
}
