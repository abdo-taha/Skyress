using MassTransit;

namespace Skyress.Application.Checkout.Sagas;

public class CheckoutSagaData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public long BasketId { get; set; }
    public long? InvoiceId { get; set; }
    public long? PaymentId { get; set; }
    public long? UserId { get; set; }
    public string CurrentState { get; set; } = default!;
}