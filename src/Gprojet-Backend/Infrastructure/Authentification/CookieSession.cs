namespace Gprojet.Backend.Infrastructure.Authentification;

public static class CookieSession
{
    public const string Nom = "gp.session";

    /// <summary>Schéma par défaut (Dev/Test) : en-tête X-Dev-Matricule si présent, sinon cookie de session.</summary>
    public const string SchemaCombine = "Gp";
}
