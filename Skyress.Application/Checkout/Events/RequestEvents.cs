namespace Skyress.Application.Checkout.Events;

// TODO separate

public record ReserveItemsRequested(Guid CorrelationId, long BasketId);

public record InitiateInvoiceRequested(Guid CorrelationId, long BasketId);

public record BuildInvoiceRequested(Guid CorrelationId, long InvoiceId, long BasketId);

public record CreatePaymentRequested(Guid CorrelationId, long InvoiceId);

public record FinalizeCheckoutRequested(Guid CorrelationId, long InvoiceId, long BasketId);