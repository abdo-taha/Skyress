using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Enums;
using Skyress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Skyress.Infrastructure.Repository
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(SkyressDbContext skyressDbContext) : base(skyressDbContext)
        {
        }

        public async Task<List<Tag>> GetTagsByTypeAsync(TagType type, CancellationToken cancellationToken)
        {
            return await GetAsync(predicate: t => t.Type == type).ToListAsync(cancellationToken);
        }
    }
}
