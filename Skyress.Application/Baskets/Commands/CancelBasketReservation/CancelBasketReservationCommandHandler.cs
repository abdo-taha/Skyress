using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Baskets.Commands.CancelBasketReservation;

public sealed class CancelBasketReservationCommandHandler : ICommandHandler<CancelBasketReservationCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IItemRepository _itemRepository;

    public CancelBasketReservationCommandHandler(IBasketRepository basketRepository, IItemRepository itemRepository)
    {
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
    }

    public async Task<Result> Handle(CancelBasketReservationCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketWithItemsAsync(request.BasketId);
        if (basket is null)
        {
            return Result.Failure(new Error("Basket.NotFound", "The basket was not found."));
        }

        if (basket.State != BasketState.Reserved)
        {
            return Result.Failure(new Error("Basket.InvalidState", "Basket is not in reserved or checked out state for cancellation."));
        }


        var itemIds = basket.BasketItems.Select(bi => bi.ItemId).ToList();
        var items = (await _itemRepository.GetByIdsAsync(itemIds)).ToDictionary(item => item.Id);


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

        await _basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
} 