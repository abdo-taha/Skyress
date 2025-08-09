namespace Skyress.API.DTOs.Baskets;

public class CheckOutBasketRequest
{
    public long BasketId { get; set; }
    public Guid? IdempotencyKey { get; set; }
}
