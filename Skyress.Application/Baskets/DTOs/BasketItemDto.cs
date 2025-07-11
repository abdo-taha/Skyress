using Skyress.Domain.Enums;

namespace Skyress.Application.Baskets.DTOs;

public class BasketItemDto
{
    public long Id { get; set; }
    public long ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Name { get; set; }
    public Unit? Unit { get; set; }
}