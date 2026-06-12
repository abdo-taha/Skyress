namespace Skyress.Domain.Exceptions;

public sealed class PaymentInvalidStateException(string message)
    : DomainException("Payment.InvalidState", message);

public sealed class PaymentInvalidAmountException(string message)
    : DomainException("Payment.InvalidAmount", message);
