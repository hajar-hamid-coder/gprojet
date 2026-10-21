using System.Text.RegularExpressions;
using FluentAssertions;

namespace Gprojet.Front.Tests;

[Trait("Category", "Mouvement")]
public sealed partial class MouvementTests
{
    private static readonly string Front = Path.Combine(Racine(), "src", "Gprojet-Front");
    private static readonly string Wwwroot = Path.Combine(Front, "wwwroot");

    private static string Racine()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null && !File.Exists(Path.Combine(d.FullName, "CLAUDE.md")))
        {
            d = d.Parent;
        }

        return d?.FullName ?? throw new InvalidOperationException("Racine du dépôt introuvable.");
    }

    [GeneratedRegex(@"(?<![\w.-])(\d*\.?\d+m?s)\b")]
    private static partial Regex Duree();

    [GeneratedRegex(@"@media\s*\(prefers-reduced-motion:\s*reduce\)\s*\{(?<corps>.*?)\n\}", RegexOptions.Singleline)]
    private static partial Regex BlocReduit();

    [Fact]
    public void Un_bloc_reduced_motion_neutralise_transition_et_animation()
    {
        var css = File.ReadAllText(Path.Combine(Wwwroot, "css", "app.gp.css"));
        var blocs = BlocReduit().Matches(css);
        blocs.Should().ContainSingle();
        blocs[0].Groups["corps"].Value.Should().Contain("animation: none").And.Contain("transition: none");
    }

    [Fact]
    public void Seules_les_durees_de_la_charte_sont_utilisees()
    {
        var fichiers = Directory.GetFiles(Path.Combine(Wwwroot, "css"), "*.css", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles(Front, "*.razor.css", SearchOption.AllDirectories)
                .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")));
        var autorisees = new[] { "0.1s", "0.15s", "0.25s", "0.6s" };
        var trouvees = fichiers.SelectMany(f => File.ReadAllLines(f)
                .Where(l => Regex.IsMatch(l, @"transition|animation"))
                .SelectMany(l => Duree().Matches(l).Select(m => m.Groups[1].Value)))
            .Distinct().ToList();
        trouvees.Should().NotBeEmpty();
        trouvees.Should().OnlyContain(d => autorisees.Contains(d));
    }

    [Fact]
    public void Aucune_bibliotheque_n_est_ajoutee_dans_lib()
    {
        var dossiers = Directory.GetDirectories(Path.Combine(Wwwroot, "lib")).Select(d => Path.GetFileName(d));
        dossiers.Should().BeEquivalentTo("fontawesome-free-6.7.2", "chartjs-4.5.1");
    }
}
