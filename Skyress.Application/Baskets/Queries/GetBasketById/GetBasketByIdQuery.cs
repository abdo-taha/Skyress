using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Baskets.Responses;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Queries.GetBasketById;

public record GetBasketByIdQuery(long Id) : IQuery<BasketResponse>;

public class GetBasketByIdQueryHandler(IBasketRepository basketRepository, ILogger<GetBasketByIdQueryHandler> logger) : IQueryHandler<GetBasketByIdQuery, BasketResponse>
{
    private readonly ILogger<GetBasketByIdQueryHandler> _logger = logger;

    public async Task<Result<BasketResponse>> Handle(GetBasketByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Query} for BasketId: {Id}", nameof(GetBasketByIdQuery), request.Id);

        var basket = await basketRepository.GetBasketWithItemsAsync(request.Id);

        if (basket is null)
        {
            return Result<BasketResponse>.Failure(Error.Dummy);
        }

        _logger.LogInformation("{Query} completed. BasketId: {Id}", nameof(GetBasketByIdQuery), request.Id);
        return Result.Success(BasketResponse.FromDomain(basket));
    }
}