using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository
{
    public class ItemRepository : GenericRepository<Item>, IItemRepository
    {
        public ItemRepository(SkyressDbContext skyressDbContext) : base(skyressDbContext)
        {
        }
    }
}
