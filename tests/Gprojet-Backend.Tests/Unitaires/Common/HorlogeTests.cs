using FluentAssertions;
using Gprojet.Backend.Common;

namespace Gprojet.Backend.Tests.Unitaires.Common;

[Trait("Category", "Common")]
public class HorlogeTests
{
    private sealed class HorlogeFactice(DateTimeOffset now) : IHorloge
    {
        public DateTimeOffset Maintenant => now;
        public DateOnly Aujourdhui => DateOnly.FromDateTime(now.UtcDateTime);
    }

    [Fact]
    public void Horloge_factice_est_figee()
    {
        IHorloge h = new HorlogeFactice(new DateTimeOffset(2026, 3, 1, 23, 30, 0, TimeSpan.Zero));
        h.Aujourdhui.Should().Be(new DateOnly(2026, 3, 1));
    }

    [Fact]
    public void Horloge_systeme_est_coherente()
    {
        var h = new HorlogeSysteme();
        h.Maintenant.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    private sealed class TempsFactice(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    [Fact]
    public void Aujourdhui_suit_le_fuseau_metier()
    {
        var h = new HorlogeSysteme(
            new TempsFactice(new DateTimeOffset(2026, 10, 16, 23, 30, 0, TimeSpan.Zero)),
            // Fuseau fixe +1 : les données Windows de Africa/Casablanca s'arrêtent en 2025 (voir HorlogeSysteme).
            TimeZoneInfo.CreateCustomTimeZone("test", TimeSpan.FromHours(1), "test", "test"));
        h.Aujourdhui.Should().Be(new DateOnly(2026, 10, 17));
    }
}
