using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Enums;

namespace Skyress.Application.Contracts.Persistence
{
    public interface ITagRepository : IGenericRepository<Tag>
    {
        Task<List<Tag>> GetTagsByTypeAsync(TagType type, CancellationToken cancellationToken);
    }
}
