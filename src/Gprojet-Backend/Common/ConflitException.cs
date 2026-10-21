namespace Gprojet.Backend.Common;

/// <summary>Conflit de concurrence ou d'unicité (HTTP 409).</summary>
public sealed class ConflitException(string message) : Exception(message);
