using System.Globalization;
using System.Text.RegularExpressions;
using FluentAssertions;

namespace Gprojet.Front.Tests;

[Trait("Category", "Charte")]
public sealed partial class CharteTests
{
    private static readonly string Front = Path.Combine(Racine(), "src", "Gprojet-Front");
    private static readonly string Css = Path.Combine(Front, "wwwroot", "css");

    private static readonly string[] Dossiers = ["Pages", "Components", "Layouts"];

    private static string Racine()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null && !File.Exists(Path.Combine(d.FullName, "CLAUDE.md")))
        {
            d = d.Parent;
        }

        return d?.FullName ?? throw new InvalidOperationException("Racine du dépôt introuvable.");
    }

    [GeneratedRegex(@"--(\w+)-text:\s*(#[0-9A-Fa-f]{6})")]
    private static partial Regex VariableTexte();

    [GeneratedRegex(@"#[0-9A-Fa-f]{3,8}\b")]
    private static partial Regex Hexa();

    [GeneratedRegex(@"--(success|warning|danger|info|purple):\s*(#[0-9A-Fa-f]{6})")]
    private static partial Regex CouleurOrigine();

    [Fact]
    public void Chaque_variable_de_texte_a_un_contraste_d_au_moins_4_5_sur_le_fond_du_badge()
    {
        var textes = VariableTexte().Matches(File.ReadAllText(Path.Combine(Css, "app.gp.css")))
            .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);
        textes.Keys.Should().BeEquivalentTo("success", "warning", "danger", "info", "purple");

        var origines = CouleurOrigine().Matches(File.ReadAllText(Path.Combine(Css, "app.css")) + File.ReadAllText(Path.Combine(Css, "app.gp.css")))
            .GroupBy(m => m.Groups[1].Value).ToDictionary(g => g.Key, g => g.First().Groups[2].Value);

        foreach (var (nom, texte) in textes)
        {
            // Fond du badge = couleur d'origine à 8 % sur blanc (voir app.css : rgba(..., 0.08)).
            var fond = Rgb(origines[nom]).Select(c => c * 0.08 + 255 * 0.92).ToArray();
            Contraste(Rgb(texte), fond).Should().BeGreaterThanOrEqualTo(4.5, $"--{nom}-text {texte}");
        }
    }

    [Fact]
    public void Aucune_couleur_hexadecimale_hors_des_cinq_variables_de_texte()
    {
        var fichiers = Dossiers
            .Select(d => Path.Combine(Front, d)).Where(Directory.Exists)
            .SelectMany(d => Directory.EnumerateFiles(d, "*.*", SearchOption.AllDirectories))
            .Where(f => f.EndsWith(".razor", StringComparison.Ordinal) || f.EndsWith(".css", StringComparison.Ordinal))
            .Append(Path.Combine(Css, "app.gp.css"));

        var fautes = fichiers.SelectMany(f => File.ReadAllLines(f)
            .Where(l => Hexa().IsMatch(l) && !VariableTexte().IsMatch(l))
            .Select(l => $"{Path.GetFileName(f)}: {l.Trim()}"));
        fautes.Should().BeEmpty();
    }

    [Fact]
    public void Aucune_url_externe_dans_index_html_ni_dans_le_css()
    {
        var fichiers = Directory.EnumerateFiles(Css, "*.css").Append(Path.Combine(Front, "wwwroot", "index.html"));
        var fautes = fichiers.Where(f => Regex.IsMatch(File.ReadAllText(f), "https?://")).ToList();
        fautes.Should().BeEmpty();
    }

    private static double[] Rgb(string hex) =>
        [.. Enumerable.Range(0, 3).Select(i => (double)int.Parse(hex.AsSpan(1 + 2 * i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture))];

    private static double Luminance(double[] rgb)
    {
        static double L(double c) { c /= 255; return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4); }
        return 0.2126 * L(rgb[0]) + 0.7152 * L(rgb[1]) + 0.0722 * L(rgb[2]);
    }

    private static double Contraste(double[] a, double[] b)
    {
        double la = Luminance(a), lb = Luminance(b);
        return (Math.Max(la, lb) + 0.05) / (Math.Min(la, lb) + 0.05);
    }
}
