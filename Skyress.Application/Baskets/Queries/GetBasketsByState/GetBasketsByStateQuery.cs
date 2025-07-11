using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Baskets.Queries.GetBasketsByState;

public record GetBasketsByStateQuery(BasketState State) : IQuery<IReadOnlyList<Basket>>;

public class GetBasketsByStateQueryHandler(IBasketRepository basketRepository) : IQueryHandler<GetBasketsByStateQuery, IReadOnlyList<Basket>>
{
    public async Task<Result<IReadOnlyList<Basket>>> Handle(GetBasketsByStateQuery request, CancellationToken cancellationToken)
    {
        var baskets = await basketRepository.GetByStateAsync(request.State);
        return Result.Success(baskets);
    }
} 