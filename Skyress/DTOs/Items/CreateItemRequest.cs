using Skyress.Domain.Enums;

namespace Skyress.API.DTOs.Items;

public record CreateItemRequest(
    string Name,
    string Description,
    double Price,
    Unit Unit,
    int QuantityLeft = 0,
    double? CostPrice = null,
    string? QrCode = null
);