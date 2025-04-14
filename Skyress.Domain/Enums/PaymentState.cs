namespace Skyress.Domain.Enums;

public enum PaymentState
{
    Paid = 0,
    Refunded = 1,
    PartiallyPaid = 2,
    Overdue = 3,
}
