namespace Gprojet.Backend.Common;

/// <summary>Résultat d'une opération sans valeur : succès, ou échec avec un message.</summary>
public class Result
{
    protected Result(bool isSuccess, string? erreur)
    {
        IsSuccess = isSuccess;
        Erreur = erreur;
    }

    public bool IsSuccess { get; }

    public string? Erreur { get; }

    public static Result Succes() => new(true, null);

    public static Result Echec(string erreur) => new(false, erreur);

    public static Result<T> Succes<T>(T valeur) => new(valeur, null);

    public static Result<T> Echec<T>(string erreur) => new(default, erreur);
}
