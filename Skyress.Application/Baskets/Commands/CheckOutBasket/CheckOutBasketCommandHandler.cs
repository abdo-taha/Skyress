using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.CheckOutBasket;

public sealed class CheckOutBasketCommandHandler : ICommandHandler<CheckOutBasketCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IItemRepository _itemRepository;

    public CheckOutBasketCommandHandler(IBasketRepository basketRepository, IItemRepository itemRepository)
    {
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
    }

    public async Task<Result> Handle(CheckOutBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetByIdAsync(request.BasketId);
        if (basket == null)
        {
            return Result.Failure(Error.Dummy);
        }
        var itemIds = basket.BasketItems.Select(bi => bi.ItemId).ToList();
        var items = (await _itemRepository.GetByIdsAsync(itemIds)).ToDictionary(item => item.Id);

        foreach (var basketItem in basket.BasketItems)
        {
            var item = items[basketItem.ItemId];
            item.MarkAsSold(basketItem.Quantity);
        }
        return Result.Success();
    }
}