namespace Gprojet.Backend.Common;

/// <summary>Demande de page ; page et taille sont bornées silencieusement (1 ≤ page ≤ int.MaxValue / 200 pour que Decalage ne déborde pas, 1 ≤ taille ≤ 200).</summary>
public sealed record PageRequete
{
    public const int TailleMax = 200;
    public const int TailleParDefaut = 25;

    private readonly int _page = 1;
    private readonly int _taille = TailleParDefaut;

    public int Page { get => _page; init => _page = Math.Clamp(value, 1, int.MaxValue / TailleMax); }

    public int Taille { get => _taille; init => _taille = Math.Clamp(value, 1, TailleMax); }

    public string? Tri { get; init; }

    public bool TriDescendant { get; init; }

    public string? Filtre { get; init; }

    public int Decalage => (Page - 1) * Taille;
}
