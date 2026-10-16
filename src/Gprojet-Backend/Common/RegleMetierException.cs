namespace Gprojet.Backend.Common;

/// <summary>Une règle métier est violée (HTTP 422).</summary>
public sealed class RegleMetierException(string message) : Exception(message);
