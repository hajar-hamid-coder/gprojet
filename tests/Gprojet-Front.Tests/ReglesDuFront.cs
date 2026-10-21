using System.Reflection;
using System.Text.RegularExpressions;

namespace Gprojet.Front.Tests
{

    /// <summary>Le front ne référence aucun code du backend, hors les DTO de Contracts liés à la compilation.</summary>
    public static partial class ReglesDuFront
    {
        private const string Backend = "Gprojet.Backend";
        private const string Contracts = "Gprojet.Backend.Contracts";

        public static IReadOnlyList<string> TypesInterdits(IEnumerable<Type> types) =>
            [.. types.Where(t => t.Namespace is { } ns && (ns == Backend || ns.StartsWith(Backend + ".", StringComparison.Ordinal))
                             && ns != Contracts && !ns.StartsWith(Contracts + ".", StringComparison.Ordinal))
                 .Select(t => t.FullName!)];

        public static bool ReferenceLeBackend(string csproj) => ReferenceProjet().IsMatch(csproj);

        public static IEnumerable<Type> TypesDe(Assembly a) => a.GetTypes();

        [GeneratedRegex(@"<ProjectReference[^>]*Gprojet-Backend", RegexOptions.IgnoreCase)]
        private static partial Regex ReferenceProjet();
    }
}

// Violation simulée pour le garde-fou (jamais dans l'assembly front).
namespace Gprojet.Backend.Services
{
    internal sealed class ServiceQuiFuiteDansLeFront;
}
