using Gprojet.Backend.Common;
using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Repositories.Organisation;

/// <summary>Repository de l'agrégat Compétence (référentiel simple, premier exemple du motif ; P4 l'étendra).</summary>
public interface ICompetenceRepository
{
    Task<Competence?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<PageResultat<Competence>> ListerAsync(PageRequete page, CancellationToken ct = default);

    void Ajouter(Competence competence);

    void Supprimer(Competence competence);
}
