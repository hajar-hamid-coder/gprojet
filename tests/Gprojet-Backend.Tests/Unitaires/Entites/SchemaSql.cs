using System.Text.RegularExpressions;

namespace Gprojet.Backend.Tests.Unitaires.Entites;

/// <summary>Lecture minimale de database/01-schema.sql : tables et contraintes <c>CHECK (col IN ('…'))</c>.</summary>
public static partial class SchemaSql
{
    public static readonly IReadOnlyDictionary<string, string> Modules = new Dictionary<string, string>
    {
        ["org"] = "Organisation",
        ["projet"] = "Projets",
        ["planif"] = "Planification",
        ["temps"] = "Temps",
        ["commun"] = "Commun",
        ["modelisation"] = "Modelisation",
    };

    private static readonly Lazy<string> Contenu = new(() => File.ReadAllText(Chemin()));

    /// <summary>(schéma, table, corps) de chaque CREATE TABLE.</summary>
    public static IEnumerable<(string Schema, string Table, string Corps)> Tables() =>
        CreateTable().Matches(Contenu.Value).Select(m => (m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value));

    /// <summary>CHECK IN à valeurs texte : (schéma, table, colonne, valeurs dans l'ordre SQL).</summary>
    public static IEnumerable<(string Schema, string Table, string Colonne, string[] Valeurs)> ChecksIn() =>
        from t in Tables()
        from Match m in CheckIn().Matches(t.Corps)
        let valeurs = Quote().Matches(m.Groups[2].Value).Select(v => v.Groups[1].Value).ToArray()
        where valeurs.Length > 0 // les CHECK IN numériques (patte.card_min IN (0,1)) restent des entiers
        select (t.Schema, t.Table, m.Groups[1].Value, valeurs);

    /// <summary>table_snake → TablePascal (règle de nommage des entités et des propriétés).</summary>
    public static string Pascal(string snake) =>
        string.Concat(snake.Split('_').Select(p => char.ToUpperInvariant(p[0]) + p[1..]));

    private static string Chemin()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Gprojet.slnx")))
            {
                return Path.Combine(dir.FullName, "database", "01-schema.sql");
            }
        }

        throw new FileNotFoundException("Gprojet.slnx introuvable en remontant depuis " + AppContext.BaseDirectory);
    }

    [GeneratedRegex(@"CREATE TABLE (\w+)\.(\w+) \((.*?)\n\);", RegexOptions.Singleline)]
    private static partial Regex CreateTable();

    [GeneratedRegex(@"CHECK \((\w+)\s+IN \(([^)]*)\)\)")]
    private static partial Regex CheckIn();

    [GeneratedRegex("'([^']*)'")]
    private static partial Regex Quote();
}
