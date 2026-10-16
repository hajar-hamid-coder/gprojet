using Bunit;
using FluentAssertions;
using Gprojet.Front.Api;
using Gprojet.Front.Components;
using Gprojet.Front.Services;
using Gprojet.Front.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Gprojet.Front.Tests;

[Trait("Category", "Etat")]
public sealed class EtatTests : BunitContext
{
    public EtatTests()
    {
        Services.AddLocalization();
        System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("fr-MA");
    }

    private GestionnaireErreursUi Gestionnaire(NotificationUiService n) =>
        new(n, new Microsoft.Extensions.Logging.Abstractions.NullLogger<GestionnaireErreursUi>(), Services.GetRequiredService<Microsoft.Extensions.Localization.IStringLocalizer<Gprojet.Front.Resources.Textes>>());

    // Composant abonné à PerimetreState, comme le fera une page.
    private sealed class AfficheurPerimetre : ComponentBase, IDisposable
    {
        [Inject] public PerimetreState Etat { get; set; } = default!;

        protected override void OnInitialized() => Etat.Changed += Rafraichir;

        private void Rafraichir() => _ = InvokeAsync(StateHasChanged);

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            b.OpenElement(0, "p");
            b.AddContent(1, $"{Etat.Niveau}|{Etat.Chemin}");
            b.CloseElement();
        }

        public void Dispose() => Etat.Changed -= Rafraichir;
    }

    [Fact]
    public void Un_composant_abonne_au_perimetre_se_rafraichit_au_changement()
    {
        var etat = new PerimetreState();
        Services.AddSingleton(etat);
        var cut = Render<AfficheurPerimetre>();
        cut.Find("p").TextContent.Should().Be("|");

        etat.Definir(Guid.NewGuid(), "division", "dir.div1");

        cut.WaitForAssertion(() => cut.Find("p").TextContent.Should().Be("division|dir.div1"));
    }

    [Fact]
    public void Une_ApiException_403_affiche_Acces_refuse_dans_le_snackbar()
    {
        var notifications = new NotificationUiService();
        Services.AddSingleton(notifications);
        var gestionnaire = Gestionnaire(notifications);
        var cut = Render<Snackbar>();

        gestionnaire.Gerer(new ApiException(403, "Forbidden", "detail interne"));

        cut.WaitForAssertion(() =>
        {
            cut.Find("[role=alert]").TextContent.Should().Contain("Accès refusé");
            cut.Markup.Should().NotContain("detail interne");
        });
    }

    [Fact]
    public void Le_message_disparait_apres_le_delai_et_peut_etre_ferme_a_la_main()
    {
        var notifications = new NotificationUiService { Delai = TimeSpan.FromMilliseconds(50) };
        Services.AddSingleton(notifications);
        var cut = Render<Snackbar>();

        notifications.Succes("Enregistré");
        cut.WaitForAssertion(() => cut.Find("[role=status]").TextContent.Should().Contain("Enregistré"));
        cut.WaitForAssertion(() => cut.FindAll(".snackbar").Should().BeEmpty());

        notifications.Delai = TimeSpan.FromMinutes(1);
        notifications.Info("Bonjour");
        cut.WaitForAssertion(() => cut.FindAll(".snackbar").Should().HaveCount(1));
        cut.Find("button[aria-label='Fermer la notification']").Click();
        cut.FindAll(".snackbar").Should().BeEmpty();
    }

    [Fact]
    public void L_indicateur_de_chargement_est_visible_pendant_l_operation_seulement()
    {
        var chargement = new ChargementService();
        Services.AddSingleton(chargement);
        var cut = Render<IndicateurChargement>();
        cut.FindAll(".chargement-global").Should().BeEmpty();

        var a = chargement.Demarrer();
        var b = chargement.Demarrer();
        cut.WaitForAssertion(() => cut.FindAll(".chargement-global").Should().HaveCount(1));

        a.Dispose();
        a.Dispose(); // idempotent
        cut.FindAll(".chargement-global").Should().HaveCount(1);
        b.Dispose();
        cut.WaitForAssertion(() => cut.FindAll(".chargement-global").Should().BeEmpty());
    }

    [Theory]
    [InlineData(401, "Session expirée")]
    [InlineData(404, "introuvable")]
    [InlineData(409, "autre utilisateur")]
    [InlineData(500, "erreur est survenue")]
    public void Les_statuts_sont_traduits_en_francais(int statut, string attendu) =>
        Gestionnaire(new NotificationUiService()).Message(new ApiException(statut)).Should().Contain(attendu);
}
