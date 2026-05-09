using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Baskets.Responses;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Queries.GetBasketsByCustomer;

public record GetBasketsByCustomerQuery(long? CustomerId) : IQuery<IReadOnlyList<BasketResponse>>;

public class GetBasketsByCustomerQueryHandler(IBasketRepository basketRepository, ILogger<GetBasketsByCustomerQueryHandler> logger) : IQueryHandler<GetBasketsByCustomerQuery, IReadOnlyList<BasketResponse>>
{
    private readonly ILogger<GetBasketsByCustomerQueryHandler> _logger = logger;

    public async Task<Result<IReadOnlyList<BasketResponse>>> Handle(GetBasketsByCustomerQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Query}", nameof(GetBasketsByCustomerQuery));

        var baskets = await basketRepository.GetByCustomerIdAsync(request.CustomerId);

        _logger.LogInformation("{Query} completed", nameof(GetBasketsByCustomerQuery));
        return Result.Success(baskets.Select(BasketResponse.FromDomain).ToList() as IReadOnlyList<BasketResponse>);
    }
} 