using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Baskets.Commands.CancelBasketReservation;

public sealed class CancelBasketReservationCommandHandler(IBasketRepository basketRepository, IItemRepository itemRepository, ILogger<CancelBasketReservationCommandHandler> logger) : ICommandHandler<CancelBasketReservationCommand>
{
    private readonly ILogger<CancelBasketReservationCommandHandler> _logger = logger;

    public async Task<Result> Handle(CancelBasketReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command} for BasketId: {Id}", nameof(CancelBasketReservationCommand), request.BasketId);

        var basket = await basketRepository.GetBasketWithItemsAsync(request.BasketId);
        if (basket is null)
        {
            return Result.Failure(new Error("Basket.NotFound", "The basket was not found."));
        }

        if (basket.State != BasketState.Reserved)
        {
            return Result.Failure(new Error("Basket.InvalidState", "Basket is not in reserved or checked out state for cancellation."));
        }

        var itemIds = basket.BasketItems.Select(bi => bi.ItemId).ToList();
        var items = (await itemRepository.GetByIdsAsync(itemIds)).ToDictionary(item => item.Id);

        foreach (var basketItem in basket.BasketItems)
        {
            if (items.ContainsKey(basketItem.ItemId))
            {
                var item = items[basketItem.ItemId];
                item.ReleaseReservation(basketItem.Quantity);
            }
        }

        var cancelResult = basket.CancelCheckout();
        if (cancelResult.IsFailure)
        {
            return Result.Failure(cancelResult.Error);
        }

        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. BasketId: {Id}", nameof(CancelBasketReservationCommand), request.BasketId);
        return Result.Success();
    }
} 