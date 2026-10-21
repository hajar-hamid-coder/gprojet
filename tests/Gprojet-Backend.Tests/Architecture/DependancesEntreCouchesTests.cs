using System.Reflection;
using FluentAssertions;

namespace Gprojet.Backend.Tests.Architecture;

[Trait("Category", "Architecture")]
public class DependancesEntreCouchesTests
{
    private const string PrefixeBackend = "Gprojet.Backend";
    private const string PrefixeViolations = "Gprojet.Backend.Tests.Architecture.Violations";
    private static readonly Assembly AssemblyBackend = typeof(Program).Assembly;
    private static readonly Assembly AssemblyTests = typeof(DependancesEntreCouchesTests).Assembly;

    public static TheoryData<string> Regles => [.. ReglesDeCouches.Toutes.Keys];

    [Theory]
    [MemberData(nameof(Regles))]
    public void Le_backend_respecte_la_regle(string regle)
    {
        var resultat = ReglesDeCouches.Toutes[regle](AssemblyBackend, PrefixeBackend);

        resultat.IsSuccessful.Should().BeTrue(
            "types en violation : {0}", string.Join(", ", resultat.FailingTypeNames ?? []));
    }

    // Garde-fou : chaque règle, rejouée sur les violations simulées, échoue exactement sur les types fautifs
    // (ni règle trivialement vraie, ni règle trop large qui rejetterait les dépendances autorisées).
    [Theory]
    [InlineData("Web_ne_depend_pas_de_Services", "Web.WebQuiAppelleUnService")]
    [InlineData("Web_ne_depend_pas_de_Repositories", "Web.WebQuiAppelleUnRepository")]
    [InlineData("Web_ne_depend_pas_de_Entities", "Web.WebQuiExposeUneEntite")]
    [InlineData("Orchestration_ne_depend_pas_des_implementations_de_Repositories", "Orchestration.OrchestrateurQuiInstancieLeRepository")]
    [InlineData("Repositories_ne_depend_pas_de_Contracts_ni_Mapping", "Repositories.RepositoryQuiRenvoieUnContrat")]
    [InlineData("Services_ne_depend_pas_de_Contracts", "Services.ServiceQuiConnaitUnContrat")]
    [InlineData("Services_ne_depend_pas_de_Mapping", "Services.ServiceQuiMappe")]
    [InlineData("Services_ne_depend_pas_de_EF_Core", "Services.ServiceQuiUtiliseEfCore")]
    [InlineData("Entities_ne_depend_que_de_Common", "Entities.EntiteQuiAppelleUnService")]
    [InlineData("Contracts_ne_depend_que_de_la_BCL_et_FluentValidation", "Contracts.ContratQuiExposeUneEntite", "Contracts.ContratQuiDependDAspNet")]
    [InlineData("Dto_uniquement_dans_Contracts", "Services.ProjetEgareDto")]
    [InlineData("Controleurs_uniquement_dans_Web", "Services.PointDEntreeEgare", "Orchestration.ProjetsController")]
    public void La_regle_detecte_une_violation_volontaire(string regle, params string[] typesFautifs)
    {
        var resultat = ReglesDeCouches.Toutes[regle](AssemblyTests, PrefixeViolations);

        resultat.IsSuccessful.Should().BeFalse();
        resultat.FailingTypeNames.Should().BeEquivalentTo(typesFautifs.Select(t => $"{PrefixeViolations}.{t}"));
    }

    [Fact]
    public void Chaque_regle_a_son_garde_fou_et_il_y_en_a_au_moins_dix()
    {
        var couvertes = typeof(DependancesEntreCouchesTests)
            .GetMethod(nameof(La_regle_detecte_une_violation_volontaire))!
            .GetCustomAttributes<InlineDataAttribute>()
            .Select(d => (string)d.GetData(null!).Single()[0]);

        couvertes.Should().BeEquivalentTo(ReglesDeCouches.Toutes.Keys);
        ReglesDeCouches.Toutes.Should().HaveCountGreaterThanOrEqualTo(10);
    }
}
