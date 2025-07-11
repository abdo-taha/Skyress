using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Queries.GetBasketsByCustomer;

public record GetBasketsByCustomerQuery(long CustomerId) : IQuery<IReadOnlyList<Basket>>;

public class GetBasketsByCustomerQueryHandler(IBasketRepository basketRepository) : IQueryHandler<GetBasketsByCustomerQuery, IReadOnlyList<Basket>>
{
    public async Task<Result<IReadOnlyList<Basket>>> Handle(GetBasketsByCustomerQuery request, CancellationToken cancellationToken)
    {
        var baskets = await basketRepository.GetByCustomerIdAsync(request.CustomerId);
        return Result.Success(baskets);
    }
} 