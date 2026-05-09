using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.RemoveItemFromBasket;

public class RemoveItemFromBasketCommandHandler(IBasketRepository basketRepository, ILogger<RemoveItemFromBasketCommandHandler> logger) : ICommandHandler<RemoveItemFromBasketCommand>
{
    private readonly ILogger<RemoveItemFromBasketCommandHandler> _logger = logger;

    public async Task<Result> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(RemoveItemFromBasketCommand));

        var basket = await basketRepository.GetBasketWithItemsAsync(request.BasketId);

        if (basket is null)
        {
            return Result.Failure(new Error("Basket.NotFound", "The basket was not found."));
        }

        var result = basket.RemoveItem(request.ItemId);

        if (result.IsFailure)
        {
            return result;
        }

        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(RemoveItemFromBasketCommand));
        return Result.Success();
    }
}