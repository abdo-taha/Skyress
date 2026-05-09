using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Baskets.Responses;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.CreateBasket;

public record CreateBasketCommand(long? CustomerId) : ICommand<BasketResponse>;

public class CreateBasketCommandHandler(IBasketRepository basketRepository, ILogger<CreateBasketCommandHandler> logger)
    : ICommandHandler<CreateBasketCommand, BasketResponse>
{
    private readonly ILogger<CreateBasketCommandHandler> _logger = logger;

    public async Task<Result<BasketResponse>> Handle(CreateBasketCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(CreateBasketCommand));

        var basket = new Basket
        {
            UserId = request.CustomerId,
        };
        await basketRepository.CreateAsync(basket, cancellationToken);

        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(CreateBasketCommand), basket.Id);
        return Result.Success(BasketResponse.FromDomain(basket));
    }
}