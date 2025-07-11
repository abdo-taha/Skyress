using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Basket;

public class BasketItem(long basketId, long itemId, int quantity) : BaseEntity
{
    public long BasketId { get; private set; } = basketId;
    public long ItemId { get; private set; } = itemId;
    public int Quantity { get; private set; } = quantity;

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }
}