namespace Skyress.Application.Checkout.Events;

// TODO separate
public record CheckoutInitiated(Guid CorrelationId, long BasketId);
public record ItemsReserved(Guid CorrelationId);

public record InvoiceInitiated(Guid CorrelationId, long InvoiceId);
public record InvoiceCreated(Guid CorrelationId);

public record PaymentInitiated(Guid CorrelationId, long PaymentId);
public record CheckoutPaymentCompleted(Guid CorrelationId);

public record FinalizedCheckout(Guid CorrelationId);