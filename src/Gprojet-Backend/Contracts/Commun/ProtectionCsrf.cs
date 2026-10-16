namespace Gprojet.Backend.Contracts.Commun;

/// <summary>En-tête exigé sur toute requête mutante portant le cookie de session (partagé serveur / front).</summary>
public static class ProtectionCsrf
{
    public const string EnTete = "X-CSRF";
    public const string Valeur = "1";
}
