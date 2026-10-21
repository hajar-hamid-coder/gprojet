namespace Gprojet.Backend.Common;

/// <summary>Horloge réelle : Maintenant en UTC, Aujourdhui dans le fuseau métier (Africa/Casablanca par défaut).</summary>
// shortcut: sous Windows les règles du Maroc ne couvrent pas 2026 (offset 0 au lieu de +1), Linux/Docker (tzdata) est correct ; à surveiller en cas d'hébergement Windows.
public sealed class HorlogeSysteme(TimeProvider temps, TimeZoneInfo fuseau) : IHorloge
{
    public HorlogeSysteme() : this(TimeProvider.System, TimeZoneInfo.FindSystemTimeZoneById("Africa/Casablanca"))
    {
    }

    public DateTimeOffset Maintenant => temps.GetUtcNow();

    public DateOnly Aujourdhui => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(Maintenant, fuseau).DateTime);
}
