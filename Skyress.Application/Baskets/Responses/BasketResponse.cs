namespace Skyress.Application.Baskets.Responses;

using Skyress.Application.Baskets.DTOs;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Enums;

public sealed record BasketResponse(
    long Id,
    long? UserId,
    BasketState State,
    long? InvoiceId,
    string? CheckoutId,
    IReadOnlyCollection<BasketItemDto> Items)
{
    public static BasketResponse FromDomain(Basket basket) => new(
        basket.Id,
        basket.UserId,
        basket.State,
        basket.InvoiceId,
        basket.CheckoutId,
        basket.BasketItems
            .Select(bi => new BasketItemDto { ItemId = bi.ItemId, Quantity = bi.Quantity })
            .ToList()
            .AsReadOnly());
}
