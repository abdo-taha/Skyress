namespace Skyress.Domain.Enums;

public enum PaymentState
{
    Initiated = 0,
    Paid = 1,
    Refunded = 2,
    PartiallyPaid = 3,
    Overdue = 4,
}
