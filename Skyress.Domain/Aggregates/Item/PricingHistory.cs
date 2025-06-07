using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Item;

public class PricingHistory : AggregateRoot, IAuditable
{
    public long ItemId { get;  private set; }

    public decimal OldPrice { get;  private set; }

    public decimal NewPrice { get;  private set; }

    public decimal OldCost { get;  private set; }

    public decimal NewCost { get;  private set; }

    public PricingChangeType PricingChangeType { get;  private set; }

    public string? LastEditBy { get;  private set; }

    public DateTime LastEditDate { get;  private set; }

    public DateTime CreatedAt { get;  private set; }
    
    public PricingHistory(
        long itemId,
        decimal oldPrice,
        decimal newPrice,
        decimal oldCost,
        decimal newCost,
        PricingChangeType pricingChangeType,
        string? lastEditBy,
        DateTime lastEditDate,
        DateTime createdAt)
    {
        ItemId = itemId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
        OldCost = oldCost;
        NewCost = newCost;
        PricingChangeType = pricingChangeType;
        LastEditBy = lastEditBy;
        LastEditDate = lastEditDate;
        CreatedAt = createdAt;
    }
}
