namespace Gprojet.Backend.Common;

/// <summary>La ressource demandée n'existe pas ou est hors périmètre (HTTP 404).</summary>
public sealed class IntrouvableException(string message) : Exception(message);
