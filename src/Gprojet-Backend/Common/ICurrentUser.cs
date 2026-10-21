namespace Gprojet.Backend.Common;

/// <summary>Agent authentifié de la requête courante.</summary>
public interface ICurrentUser
{
    Guid Id { get; }

    string Matricule { get; }

    Guid EntiteId { get; }

    IReadOnlyCollection<string> Roles { get; }
}
