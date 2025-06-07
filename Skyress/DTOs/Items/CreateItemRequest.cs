using Skyress.Domain.Enums;

namespace Skyress.API.DTOs.Items;

public record CreateItemRequest(
    string Name,
    string Description,
    decimal Price,
    Unit Unit,
    int QuantityLeft = 0,
    decimal? CostPrice = null,
    string? QrCode = null
);