using Skyress.Domain.Enums;

namespace Skyress.Application.Baskets.DTOs;

public class BasketDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public BasketState State { get; set; }
    public List<BasketItemDto>? Items { get; set; }
}