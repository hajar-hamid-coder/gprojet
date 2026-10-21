using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;

namespace Gprojet.Backend.Tests.Architecture;

/// <summary>
/// Règles de dépendance entre couches (dossiers / espaces de noms de Gprojet-Backend).
/// Chaque règle est paramétrée par l'assembly et le préfixe d'espace de noms racine, pour être
/// rejouée à la fois sur le backend (doit réussir) et sur les violations simulées du projet de test (doit échouer).
/// </summary>
public static class ReglesDeCouches
{
    public delegate TestResult Regle(Assembly assembly, string prefixe);

    public static readonly IReadOnlyDictionary<string, Regle> Toutes = new Dictionary<string, Regle>
    {
        ["Web_ne_depend_pas_de_Services"] = (a, p) => NeDependPas(a, p, "Web", Ns(p, "Services")),
        ["Web_ne_depend_pas_de_Repositories"] = (a, p) => NeDependPas(a, p, "Web", Ns(p, "Repositories")),
        ["Web_ne_depend_pas_de_Entities"] = (a, p) => NeDependPas(a, p, "Web", Ns(p, "Entities")),
        ["Orchestration_ne_depend_pas_des_implementations_de_Repositories"] = OrchestrationSansImplementationDeRepository,
        ["Repositories_ne_depend_pas_de_Contracts_ni_Mapping"] = (a, p) => NeDependPas(a, p, "Repositories", Ns(p, "Contracts"), Ns(p, "Mapping")),
        ["Services_ne_depend_pas_de_Contracts"] = (a, p) => NeDependPas(a, p, "Services", Ns(p, "Contracts")),
        ["Services_ne_depend_pas_de_Mapping"] = (a, p) => NeDependPas(a, p, "Services", Ns(p, "Mapping")),
        ["Services_ne_depend_pas_de_EF_Core"] = (a, p) => NeDependPas(a, p, "Services", "Microsoft.EntityFrameworkCore"),
        ["Entities_ne_depend_que_de_Common"] = (a, p) => NeDependPas(a, p, "Entities",
            [.. new[] { "Web", "Orchestration", "Services", "Repositories", "Mapping", "Contracts", "Infrastructure" }.Select(c => Ns(p, c))]),
        ["Contracts_ne_depend_que_de_la_BCL_et_FluentValidation"] = (a, p) => Types.InAssembly(a)
            .That().ResideInNamespaceMatching(Couche(p, "Contracts"))
            .Should().OnlyHaveDependenciesOn("System", "FluentValidation", Ns(p, "Contracts"))
            .GetResult(),
        ["Dto_uniquement_dans_Contracts"] = (a, p) => Types.InAssembly(a)
            .That().ResideInNamespaceMatching(Racine(p)).And().HaveNameEndingWith("Dto")
            .Should().ResideInNamespaceMatching(Couche(p, "Contracts"))
            .GetResult(),
        ["Controleurs_uniquement_dans_Web"] = (a, p) => Types.InAssembly(a)
            .That().ResideInNamespaceMatching(Racine(p)).And().Inherit(typeof(ControllerBase))
            .Or().ResideInNamespaceMatching(Racine(p)).And().HaveNameEndingWith("Controller")
            .Should().ResideInNamespaceMatching(Couche(p, "Web"))
            .GetResult(),
    };

    private static TestResult NeDependPas(Assembly a, string p, string couche, params string[] interdits) =>
        Types.InAssembly(a)
            .That().ResideInNamespaceMatching(Couche(p, couche))
            .ShouldNot().HaveDependencyOnAny(interdits)
            .GetResult();

    // NetArchTest ne distingue pas interface / classe dans les dépendances : on interdit nommément chaque type
    // concret (classe, struct, record, DbContext…) de Repositories ; les interfaces restent autorisées.
    private static TestResult OrchestrationSansImplementationDeRepository(Assembly a, string p)
    {
        var implementations = a.GetTypes()
            .Where(t => !t.IsNested && !t.IsInterface && !t.IsEnum && t.Namespace is { } ns
                        && Regex.IsMatch(ns, Couche(p, "Repositories")))
            .Select(t => t.FullName!)
            .DefaultIfEmpty("<aucune implementation>") // nom impossible : la règle reste valide sur une couche vide
            .ToArray();

        return Types.InAssembly(a)
            .That().ResideInNamespaceMatching(Couche(p, "Orchestration"))
            .ShouldNot().HaveDependencyOnAny(implementations)
            .GetResult();
    }

    // NetArchTest 1.3.2 compare les dépendances par segments d'espace de noms : pas de point final.
    private static string Ns(string prefixe, string couche) => $"{prefixe}.{couche}";

    private static string Couche(string prefixe, string couche) => $"^{Regex.Escape($"{prefixe}.{couche}")}(\\.|$)";

    private static string Racine(string prefixe) => $"^{Regex.Escape(prefixe)}(\\.|$)";
}
