using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;
using Skyress.Domain.Exceptions;

namespace Skyress.Application.Baskets.Commands.ReserveItems;

public sealed record ReserveItemsCommand(long BasketId) : ICommand; 

public class ReserveItemsCommandHandler : ICommandHandler<ReserveItemsCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IItemRepository _itemRepository;

    public ReserveItemsCommandHandler(IBasketRepository basketRepository, IItemRepository itemRepository)
    {
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
    }

    public async Task<Result> Handle(ReserveItemsCommand request, CancellationToken cancellationToken)
    {
        Basket? basket = await _basketRepository.GetBasketWithItemsAsync(request.BasketId);
        if (basket == null)
            return Result.Failure(new Error("Basket.NotFound", "The basket was not found."));

        Dictionary<long, Item> items = await GetItems(basket);
        bool anyReserved = false;

        foreach (BasketItem basketItem in basket.BasketItems)
        {
            if (basketItem.IsReserved)
                continue; // already done — skip

            if (!items.TryGetValue(basketItem.ItemId, out Item? item))
            {
                return Result.Failure(new Error("Item.NotFound", $"Item {basketItem.ItemId} was not found."));
            }

            try
            {
                item.ReserveQuantity(basketItem.Quantity);
            }
            catch (DomainException exception)
            {
                return DomainExceptionResultMapper.ToFailure(exception);
            }

            basketItem.MarkAsReserved();
            anyReserved = true;
        }

        if (anyReserved)
            await this._itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    
    private async Task<Dictionary<long, Item>> GetItems(Basket basket)
    {
        List<long> itemIds = basket.BasketItems.Select(bi => bi.ItemId).ToList();
        IReadOnlyList<Item> items = await _itemRepository.GetByIdsAsync(itemIds);
        return items.ToDictionary(item => item.Id);
    }
}
