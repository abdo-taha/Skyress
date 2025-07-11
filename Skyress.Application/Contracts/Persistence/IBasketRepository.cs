using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Enums;

namespace Skyress.Application.Contracts.Persistence;

public interface IBasketRepository : IGenericRepository<Basket>
{
    Task<IReadOnlyList<Basket>> GetByCustomerIdAsync(long customerId);
    Task<IReadOnlyList<Basket>> GetByStateAsync(BasketState state);
    Task<Basket?> GetBasketWithItemsAsync(long basketId);
}