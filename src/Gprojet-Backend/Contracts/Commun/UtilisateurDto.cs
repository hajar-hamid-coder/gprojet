namespace Gprojet.Backend.Contracts.Commun;

/// <summary>Agent connecté, tel que le front le lit via <c>/bff/user</c>.</summary>
public sealed record UtilisateurDto(Guid Id, string Matricule, Guid EntiteId, string[] Roles);
