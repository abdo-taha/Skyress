using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Item.Events;

public sealed record ItemPriceChangedDomainEvent(Guid Id, long ItemId, decimal OldPrice, decimal NewPrice, decimal OldCost, decimal NewCost, PricingChangeType PricingChangeType, string? LastEditBy, DateTime LastEditDate, DateTime CreatedAt) : IDomainEvent
{
} 