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
        if (State != BasketState.Active)
        {
            return Result.Failure(new Error("Basket.InvalidState", "Cannot add items to basket in current state."));
        }

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
        if (State != BasketState.Active)
        {
            return Result.Failure(new Error("Basket.InvalidState", "Cannot remove items from basket in current state."));
        }

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
        if (State == BasketState.Active)
        {
            _basketItems.Clear();
        }
    }

    public Result InitiateCheckout()
    {
        if (State != BasketState.Active && State != BasketState.Cancelled)
        {
            return Result.Failure(Error.Dummy);
        }

        if (!_basketItems.Any())
        {
            return Result.Failure(Error.Dummy);
        }

        State = BasketState.Reserved;
        return Result.Success();
    }
    
    public Result CompleteCheckout()
    {
        if (State != BasketState.Reserved)
        {
            return Result.Failure(Error.Dummy);
        }

        State = BasketState.CheckedOut;
        return Result.Success();
    }

    public Result CancelCheckout()
    {
        if (State != BasketState.Reserved)
        {
            return Result.Failure(Error.Dummy);
        }

        State = BasketState.Cancelled;
        return Result.Success();
    }
}