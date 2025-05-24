using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Item;

public class PricingHistory : AggregateRoot, IAuditable
{
    public long ItemId { get; set; }

    public decimal OldPrice { get; set; }

    public decimal NewPrice { get; set; }

    public decimal OldCost { get; set; }

    public decimal NewCost { get; set; }

    public PricingChangeType pricingChangeType { get; set; }

    public string? LastEditBy { get; set; }

    public DateTime LastEditDate { get; set; }

    public DateTime CreatedAt { get; init; }
}
