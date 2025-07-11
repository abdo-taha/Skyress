using Microsoft.EntityFrameworkCore;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Enums;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository;

public class BasketRepository
    : GenericRepository<Basket>, IBasketRepository
{
    protected BasketRepository(SkyressDbContext skyressDbContext) : base(skyressDbContext)
    {
    }

    public async Task<IReadOnlyList<Basket>> GetByCustomerIdAsync(long customerId)
    {
        return await GetAsync(b => b.UserId == customerId).ToListAsync();
    }

    public async Task<IReadOnlyList<Basket>> GetByStateAsync(BasketState state)
    {
        return await GetAsync(b => b.State == state).ToListAsync();
    }

    public async Task<Basket?> GetBasketWithItemsAsync(long basketId)
    {
        return await SkyressDbContext.Baskets
            .Include(b => b.BasketItems)
            .FirstOrDefaultAsync(b => b.Id == basketId);
    }
}