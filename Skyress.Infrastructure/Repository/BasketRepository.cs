using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Enums;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository;

public class BasketRepository
    : GenericRepository<Basket>, IBasketRepository
{
    public BasketRepository(SkyressDbContext skyressDbContext) : base(skyressDbContext)
    {
    }

    public async Task<IReadOnlyList<Basket>> GetByCustomerIdAsync(long? customerId)
    {
        return await GetAsync(b => b.UserId == customerId, includes: new List<Expression<Func<Basket, object>>>()
        {
            basket => basket.BasketItems
        }).ToListAsync();
    }

    public async Task<IReadOnlyList<Basket>> GetByStateAsync(BasketState state)
    {
        return await GetAsync(b => b.State == state).ToListAsync();
    }

    public async Task<Basket?> GetBasketWithItemsAsync(long basketId)
    {
        return await GetAsync(b => b.Id == basketId, includes: new List<Expression<Func<Basket, object>>>()
        {
            basket => basket.BasketItems
        }).FirstOrDefaultAsync();
    }

    public async Task<Basket?> GetBasketByPaymentIdAsync(long paymentId)
    {
        return await GetAsync( b => b.PaymentId == paymentId, includes: new List<Expression<Func<Basket, object>>>()
        {
            basket => basket.BasketItems
        }).FirstOrDefaultAsync();
    }
}