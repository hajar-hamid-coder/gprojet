namespace Gprojet.Backend.Common;

/// <summary>Une page d'éléments avec le total global.</summary>
public sealed record PageResultat<T>(IReadOnlyList<T> Elements, int Total, int Page, int Taille)
{
    public int NombrePages => Total == 0 ? 0 : (int)Math.Ceiling(Total / (double)Taille);
}
