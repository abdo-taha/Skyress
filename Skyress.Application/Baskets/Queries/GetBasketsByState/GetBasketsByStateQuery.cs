using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Baskets.Responses;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Baskets.Queries.GetBasketsByState;

public record GetBasketsByStateQuery(BasketState State) : IQuery<IReadOnlyList<BasketResponse>>;

public class GetBasketsByStateQueryHandler(IBasketRepository basketRepository, ILogger<GetBasketsByStateQueryHandler> logger) : IQueryHandler<GetBasketsByStateQuery, IReadOnlyList<BasketResponse>>
{
    private readonly ILogger<GetBasketsByStateQueryHandler> _logger = logger;

    public async Task<Result<IReadOnlyList<BasketResponse>>> Handle(GetBasketsByStateQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Query}", nameof(GetBasketsByStateQuery));

        var baskets = await basketRepository.GetByStateAsync(request.State);

        _logger.LogInformation("{Query} completed", nameof(GetBasketsByStateQuery));
        return Result.Success(baskets.Select(BasketResponse.FromDomain).ToList() as IReadOnlyList<BasketResponse>);
    }
} 