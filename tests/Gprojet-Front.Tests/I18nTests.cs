using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Bunit;
using FluentAssertions;
using Gprojet.Backend.Contracts.Commun;
using Gprojet.Front.Auth;
using Gprojet.Front.Layouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gprojet.Front.Tests;

[Trait("Category", "I18n")]
public sealed partial class I18nTests : BunitContext
{
    private sealed class Fournisseur : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
    }

    private readonly CultureInfo _avant = CultureInfo.CurrentUICulture;

    protected override void Dispose(bool disposing)
    {
        CultureInfo.CurrentUICulture = _avant;
        base.Dispose(disposing);
    }

    private IRenderedComponent<MainLayout> Rendre(string culture)
    {
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
        Services.AddLocalization();
        Services.RemoveAll<IAuthorizationService>();
        Services.AddAuthorizationCore(Politiques.Enregistrer);
        Services.AddSingleton(new Gprojet.Front.Api.BffApiClient(new HttpClient { BaseAddress = new Uri("http://localhost/") }));
        Services.AddSingleton<Gprojet.Front.Services.NotificationUiService>();
        Services.AddSingleton<Gprojet.Front.Services.ChargementService>();
        Services.AddCascadingAuthenticationState();
        Services.AddSingleton<AuthenticationStateProvider>(new Fournisseur());
        return Render<MainLayout>(p => p.Add(l => l.Body, "<p>contenu</p>"));
    }

    [Fact]
    public void En_ar_MA_le_layout_est_rendu_avec_dir_rtl()
    {
        Rendre("ar-MA").Find(".page").GetAttribute("dir").Should().Be("rtl");
    }

    [Theory]
    [InlineData("fr-MA", "ltr")]
    [InlineData("en", "ltr")]
    public void Hors_arabe_le_layout_reste_ltr(string culture, string dir)
    {
        Rendre(culture).Find(".page").GetAttribute("dir").Should().Be(dir);
    }

    [Theory]
    [InlineData("fr-MA", "Mes tâches")]
    [InlineData("en", "My tasks")]
    [InlineData("ar-MA", "مهامي")]
    public void Un_libelle_du_menu_change_selon_la_culture(string culture, string attendu)
    {
        Rendre(culture).Find("a[href='taches']").TextContent.Trim().Should().Be(attendu);
    }

    [Fact]
    public void Le_selecteur_de_langue_propose_la_liste_blanche_et_marque_la_culture_courante()
    {
        var cut = Rendre("en");

        cut.FindAll("select.langue-select option").Select(o => o.GetAttribute("value")).Should().Equal(Langues.Cultures);
        cut.Find("select.langue-select").GetAttribute("aria-label").Should().Be("Language");
    }

    [Fact]
    public void Dates_et_nombres_suivent_la_culture()
    {
        var fr = CultureInfo.GetCultureInfo("fr-MA");
        var en = CultureInfo.GetCultureInfo("en");

        1234.5.ToString("N1", fr).Should().EndWith(",5").And.NotContain("1,234");
        1234.5.ToString("N1", en).Should().Be("1,234.5");
        new DateTime(2026, 3, 9).ToString("d", fr).Should().Be("09/03/2026");
        new DateTime(2026, 3, 9).ToString("d", en).Should().NotBe("09/03/2026");
        CultureInfo.GetCultureInfo("ar-MA").TextInfo.IsRightToLeft.Should().BeTrue();
    }

    // ---- Garde-fou : aucun texte français en dur dans les .razor (layout, composants, pages, App) ----

    [GeneratedRegex(@"@\*.*?\*@", RegexOptions.Singleline)]
    private static partial Regex CommentaireRazor();

    [GeneratedRegex(@">([^<>]+)<")]
    private static partial Regex TexteEntreBalises();

    [GeneratedRegex(@"@[\w.?]+(\[[^\]]*\]|\([^)]*\))*")]
    private static partial Regex ExpressionRazor();

    [GeneratedRegex(@"\b(?:title|aria-label|placeholder|alt|Libelle)=""([^""@]*)""")]
    private static partial Regex AttributTexte();

    [GeneratedRegex(@"\p{L}{2,}")]
    private static partial Regex Lettres();

    private static readonly string[] NonTraduisibles = ["Ctrl+K"];
    private static readonly string[] Dossiers = ["Layouts", "Components", "Pages"];

    internal static IEnumerable<string> TextesEnDur(string razor)
    {
        var i = razor.IndexOf("@code", StringComparison.Ordinal);
        var vue = CommentaireRazor().Replace(i < 0 ? razor : razor[..i], "");

        foreach (Match m in TexteEntreBalises().Matches(vue))
        {
            var texte = string.Join(' ', m.Groups[1].Value.Split('\n')
                .Select(l => l.Trim()).Where(l => l.Length > 0 && l[0] is not ('@' or '{' or '}') && !l.StartsWith("else", StringComparison.Ordinal)));
            texte = ExpressionRazor().Replace(texte, "").Trim();
            if (Lettres().IsMatch(texte) && !NonTraduisibles.Contains(texte)) yield return texte;
        }

        foreach (Match m in AttributTexte().Matches(vue))
            if (Lettres().IsMatch(m.Groups[1].Value)) yield return m.Groups[1].Value;
    }

    private static string Racine()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null && !File.Exists(Path.Combine(d.FullName, "CLAUDE.md"))) d = d.Parent;
        return d?.FullName ?? throw new InvalidOperationException("Racine du dépôt introuvable.");
    }

    [Fact]
    public void Aucun_texte_en_dur_dans_les_razor_du_front()
    {
        var front = Path.Combine(Racine(), "src", "Gprojet-Front");
        var fichiers = Dossiers
            .SelectMany(d => Directory.GetFiles(Path.Combine(front, d), "*.razor", SearchOption.AllDirectories))
            .Append(Path.Combine(front, "App.razor")).ToList();
        fichiers.Should().Contain(f => f.EndsWith("MainLayout.razor", StringComparison.Ordinal));

        var fautes = fichiers.SelectMany(f => TextesEnDur(File.ReadAllText(f)).Select(t => $"{Path.GetFileName(f)} : {t}")).ToList();

        fautes.Should().BeEmpty("tout texte visible vient de Resources/Textes*.resx");
    }

    [Fact]
    public void Le_garde_fou_detecte_une_chaine_francaise_simulee()
    {
        TextesEnDur("<div><span>Aller au contenu</span></div>").Should().Equal("Aller au contenu");
        TextesEnDur("<button aria-label=\"Se déconnecter\" title=\"@T[\"x\"]\"></button>").Should().Equal("Se déconnecter");
        TextesEnDur("<input placeholder=\"Rechercher…\" />").Should().Equal("Rechercher…");
        TextesEnDur("<span>@T[\"Marque\"]</span>\n@if (x)\n{\n<i class=\"a\"></i>\n}").Should().BeEmpty();
    }

    [Fact]
    public void Toutes_les_cles_existent_dans_les_trois_resx()
    {
        var dossier = Path.Combine(Racine(), "src", "Gprojet-Front", "Resources");
        string[] Cles(string f) => [.. Regex.Matches(File.ReadAllText(Path.Combine(dossier, f)), @"<data name=""(\w+)""").Select(m => m.Groups[1].Value).Order()];

        Cles("Textes.en.resx").Should().Equal(Cles("Textes.resx"));
        Cles("Textes.ar.resx").Should().Equal(Cles("Textes.resx"));
    }
}
