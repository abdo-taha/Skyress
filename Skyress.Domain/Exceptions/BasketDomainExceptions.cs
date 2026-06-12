namespace Skyress.Domain.Exceptions;

public sealed class BasketInvalidStateException(string message)
    : DomainException("Basket.InvalidState", message);

public sealed class BasketEmptyException()
    : DomainException("Basket.Empty", "Cannot checkout an empty basket.");

public sealed class BasketAlreadyCheckedOutException()
    : DomainException("Basket.AlreadyCheckedOut", "Basket has already been checked out.");
