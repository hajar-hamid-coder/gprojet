namespace Gprojet.Backend.Contracts.Commun;

/// <summary>Cultures prises en charge (liste blanche partagée serveur / front) et nom du cookie qui mémorise le choix.</summary>
public static class Langues
{
    public const string NomCookie = "gp.culture";
    public const string Defaut = "fr-MA";

    public static readonly IReadOnlyList<string> Cultures = [Defaut, "en", "ar-MA"];
}

/// <summary>Corps de POST /bff/langue.</summary>
public sealed record LangueDto(string Culture);
