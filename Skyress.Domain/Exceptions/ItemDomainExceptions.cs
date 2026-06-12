namespace Skyress.Domain.Exceptions;

public sealed class ItemInsufficientStockException(int available, int requested)
    : DomainException(
        "Item.InsufficientStock",
        $"Insufficient available stock. Available: {available}, Requested: {requested}");

public sealed class ItemInvalidReservationException(int reserved, int requested)
    : DomainException(
        "Item.InvalidReservation",
        $"Cannot release more than reserved. Reserved: {reserved}, Requested: {requested}");

public sealed class ItemInvalidSaleException()
    : DomainException("Item.InvalidSale", "Cannot mark more items as sold than are available or reserved.");
