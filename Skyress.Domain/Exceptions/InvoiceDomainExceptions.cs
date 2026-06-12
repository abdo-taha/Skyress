namespace Skyress.Domain.Exceptions;

public sealed class InvoiceInvalidStateException(string message)
    : DomainException("Invoice.InvalidState", message);

public sealed class InvoicePaymentAlreadyAttachedException()
    : DomainException("Invoice.PaymentAlreadyAttached", "Invoice already has a payment attached.");

public sealed class InvoiceDuplicateSoldItemException(long itemId)
    : DomainException("Invoice.DuplicateSoldItem", $"Invoice already contains a sold item for item {itemId}.");
