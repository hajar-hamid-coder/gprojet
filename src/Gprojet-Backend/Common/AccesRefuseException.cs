namespace Gprojet.Backend.Common;

/// <summary>L'agent n'a pas le droit d'accéder à la ressource (HTTP 403).</summary>
public sealed class AccesRefuseException(string message) : Exception(message);
