using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Gprojet.Backend.Common;
using Gprojet.Backend.Entities.Commun;

namespace Gprojet.Backend.Repositories.Audit;

/// <summary>
/// Sérialisation canonique et empreinte d'une ligne de <c>commun.historique</c> (écriture ET vérification).
/// Contenu = champs dans un ordre fixe, chacun préfixé par sa longueur (<c>len:valeur;</c>, <c>~;</c> pour NULL) : pas d'ambiguïté de séparateur.
/// <c>date_action</c> est incluse, en UTC ISO 8601 à la microseconde ; l'écriture la tronque à la microseconde
/// (précision de timestamptz) pour que la valeur relue en base redonne le même texte.
/// Empreinte = sha256(empreinte_precedente ou "" + "|" + contenu), hex minuscule de 64 caractères.
/// </summary>
public static class EmpreinteHistorique
{
    public static string Calculer(string? precedente, Historique h)
    {
        ArgumentNullException.ThrowIfNull(h);
        var contenu = string.Concat(
            Champ(h.TypeCible), Champ(h.CibleId.ToString()), Champ(ValeurEnumSql.VersSql(h.Action)),
            Champ(h.Champ), Champ(h.AncienneValeur), Champ(h.NouvelleValeur), Champ(h.AgentId?.ToString()),
            Champ(h.DateAction.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'", CultureInfo.InvariantCulture)),
            Champ(h.AdresseIp?.ToString()), Champ(h.CorrelationId?.ToString()));
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes($"{precedente}|{contenu}")));
    }

    /// <summary>Tronque à la microseconde (précision de PostgreSQL).</summary>
    public static DateTimeOffset Microseconde(DateTimeOffset t) => new(t.UtcTicks / 10 * 10, TimeSpan.Zero);

    private static string Champ(string? v) => v is null ? "~;" : $"{v.Length}:{v};";
}
