namespace Skyress.API.DTOs.Baskets;

public class InitiateCheckoutRequest
{
    public long BasketId { get; set; }
    public Guid? IdempotencyKey { get; set; }
}
