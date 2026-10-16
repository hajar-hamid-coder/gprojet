namespace Gprojet.Backend.Common;

/// <summary>Résultat d'une opération portant une valeur en cas de succès.</summary>
public sealed class Result<T> : Result
{
    private readonly T? _valeur;

    internal Result(T? valeur, string? erreur) : base(erreur is null, erreur) => _valeur = valeur;

    /// <summary>Valeur du succès ; lève une exception si le résultat est un échec.</summary>
    public T Valeur => IsSuccess
        ? _valeur!
        : throw new InvalidOperationException("Un résultat en échec n'a pas de valeur : " + Erreur);
}
