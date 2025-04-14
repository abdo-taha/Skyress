using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.TagAssignmnet;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository;

public class TagAssignmentRepository : GenericRepository<TagAssignment>, ITagAssignmentRepository
{
    public TagAssignmentRepository(SkyressDbContext skyressDbContext) : base(skyressDbContext)
    {
    }
}
