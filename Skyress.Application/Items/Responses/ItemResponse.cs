namespace Skyress.Application.Items.Responses;

using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Enums;

public sealed record ItemResponse(
    long Id,
    string Name,
    string? Description,
    decimal Price,
    decimal? CostPrice,
    int QuantityLeft,
    int QuantityReserved,
    int QuantitySold,
    string? QrCode,
    Unit Unit,
    bool IsDeleted,
    DateTime CreatedAt)
{
    public static ItemResponse FromDomain(Item item) => new(
        item.Id,
        item.Name,
        item.Description,
        item.Price,
        item.CostPrice,
        item.QuantityLeft,
        item.QuantityReserved,
        item.QuantitySold,
        item.QrCode,
        item.Unit,
        item.IsDeleted,
        item.CreatedAt);
}
