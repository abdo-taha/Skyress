using Skyress.Domain.Primitives;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;
using Skyress.Domain.Exceptions;

namespace Skyress.Domain.Aggregates.Basket;

public class Basket : AggregateRoot
{
    public long? UserId { get; set; }

    public BasketState State { get; private set; } = BasketState.Active;
    
    public long? InvoiceId { get; private set; }
    
    public string? CheckoutId { get; private set; }
    
    private readonly List<BasketItem> _basketItems = new();
    
    public IReadOnlyCollection<BasketItem> BasketItems => _basketItems.AsReadOnly();

    public Result AddItem(long itemId, int quantity)
    {
        if (State != BasketState.Active)
        {
            throw new BasketInvalidStateException("Cannot add items to basket in current state.");
        }

        var existingItem = _basketItems.FirstOrDefault(bi => bi.ItemId == itemId);

        if (existingItem is not null)
        {
            if (quantity > existingItem.Quantity)
            {
                throw new BasketInvalidStateException("Cannot remove more items than exist in the basket.");
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
                throw new BasketInvalidStateException("Cannot add a non-positive item quantity.");
            }
            _basketItems.Add(new BasketItem(Id, itemId, quantity));
        }

        return Result.Success();
    }

    public Result RemoveItem(long itemId)
    {
        if (State != BasketState.Active)
        {
            throw new BasketInvalidStateException("Cannot remove items from basket in current state.");
        }

        var itemToRemove = _basketItems.FirstOrDefault(bi => bi.ItemId == itemId);

        if (itemToRemove is null)
        { 
            throw new BasketInvalidStateException("Cannot remove an item that is not in the basket.");
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
            throw new BasketInvalidStateException("Cannot initiate checkout for basket in current state.");
        }

        if (!_basketItems.Any())
        {
            throw new BasketEmptyException();
        }

        State = BasketState.Reserved;
        return Result.Success();
    }

    public Guid EnsureCheckoutId()
    {
        if (Guid.TryParse(CheckoutId, out Guid checkoutId) && checkoutId != Guid.Empty)
        {
            return checkoutId;
        }

        Guid newCheckoutId = Guid.NewGuid();
        CheckoutId = newCheckoutId.ToString();
        return newCheckoutId;
    }

    public void AttachInvoice(long invoiceId)
    {
        if (InvoiceId is not null && InvoiceId != invoiceId)
        {
            throw new BasketInvalidStateException("Basket already has a different invoice attached.");
        }

        InvoiceId = invoiceId;
    }
    
    public Result CompleteCheckout()
    {
        if (State != BasketState.Reserved)
        {
            throw new BasketInvalidStateException("Cannot complete checkout for basket in current state.");
        }

        State = BasketState.CheckedOut;
        return Result.Success();
    }

    public Result CancelCheckout()
    {
        if (State != BasketState.Reserved)
        {
            throw new BasketInvalidStateException("Cannot cancel checkout for basket in current state.");
        }

        State = BasketState.Cancelled;
        return Result.Success();
    }
}
