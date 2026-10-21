using Gprojet.Backend.Common;
using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Repositories.Organisation;

internal sealed class CompetenceRepository(GpDbContext db) : RepositoryBase<Competence>(db), ICompetenceRepository
{
    public Task<PageResultat<Competence>> ListerAsync(PageRequete page, CancellationToken ct = default) => PageAsync(page, null, ct);
}
