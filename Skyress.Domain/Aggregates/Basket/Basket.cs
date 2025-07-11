using Skyress.Domain.primitives;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Domain.Aggregates.Basket;

public class Basket : AggregateRoot
{
    public long UserId { get; set; }

    public BasketState State { get; set; } = BasketState.Active;
    
    private readonly List<BasketItem> _basketItems = new();
    public IReadOnlyCollection<BasketItem> BasketItems => _basketItems.AsReadOnly();

    public Result AddItem(long itemId, int quantity)
    {
        var existingItem = _basketItems.FirstOrDefault(bi => bi.ItemId == itemId);

        if (existingItem is not null)
        {
            if (quantity > existingItem.Quantity)
            {
                return Result.Failure(Error.Dummy);
            }
            
            existingItem.AddQuantity(quantity);
            
            if (existingItem.Quantity == 0)
            {
                _basketItems.Remove(existingItem);
            }
        }
        else
        {
            if (quantity < 1)
            {
                return Result.Failure(Error.Dummy);
            }
            _basketItems.Add(new BasketItem(Id, itemId, quantity));
        }

        return Result.Success();
    }

    public Result RemoveItem(long itemId)
    {
        var itemToRemove = _basketItems.FirstOrDefault(bi => bi.ItemId == itemId);

        if (itemToRemove is null)
        { 
            return Result.Failure(Error.Dummy); 
        }

        _basketItems.Remove(itemToRemove);
        
        return Result.Success();
    }

    public void Clear()
    {
        _basketItems.Clear();
    }
}