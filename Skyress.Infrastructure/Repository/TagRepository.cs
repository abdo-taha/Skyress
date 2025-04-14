using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Tag;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(SkyressDbContext skyressDbContext) : base(skyressDbContext)
        {
        }
    }
}
