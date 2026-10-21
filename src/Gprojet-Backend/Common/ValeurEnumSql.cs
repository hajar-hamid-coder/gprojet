using System.Collections.Frozen;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Gprojet.Backend.Common;

/// <summary>
/// Conversion énumération C# ⇄ valeur texte des <c>CHECK (… IN (…))</c> SQL (règle R9, stockage texte).
/// Règle : membre PascalCase → MAJUSCULES_SNAKE (<c>EnCours</c> → <c>EN_COURS</c>) ; un <see cref="EnumMemberAttribute"/> l'emporte.
/// </summary>
public static partial class ValeurEnumSql
{
    public static string VersSql<TEnum>(TEnum valeur) where TEnum : struct, Enum => Cache<TEnum>.VersSql[valeur];

    public static TEnum DepuisSql<TEnum>(string valeur) where TEnum : struct, Enum =>
        Cache<TEnum>.DepuisSql.TryGetValue(valeur, out var resultat)
            ? resultat
            : throw new ArgumentOutOfRangeException(nameof(valeur), valeur, $"Valeur inconnue pour {typeof(TEnum).Name}.");

    /// <summary>Version non générique (réflexion, sans cache) : tests et outillage.</summary>
    public static string VersSql(Enum valeur)
    {
        ArgumentNullException.ThrowIfNull(valeur);
        var nom = valeur.ToString();
        return valeur.GetType().GetField(nom)?.GetCustomAttribute<EnumMemberAttribute>()?.Value
               ?? Majuscule().Replace(nom, "_").ToUpperInvariant();
    }

    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex Majuscule();

    private static class Cache<TEnum> where TEnum : struct, Enum
    {
        public static readonly FrozenDictionary<TEnum, string> VersSql =
            Enum.GetValues<TEnum>().ToFrozenDictionary(v => v, v => ValeurEnumSql.VersSql((Enum)v));

        public static readonly FrozenDictionary<string, TEnum> DepuisSql =
            VersSql.ToFrozenDictionary(p => p.Value, p => p.Key, StringComparer.Ordinal);
    }
}
