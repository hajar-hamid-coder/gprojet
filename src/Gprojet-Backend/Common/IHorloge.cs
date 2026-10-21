namespace Gprojet.Backend.Common;

/// <summary>Source de temps injectable (remplaçable par une horloge factice en test).</summary>
public interface IHorloge
{
    DateTimeOffset Maintenant { get; }

    DateOnly Aujourdhui { get; }
}
