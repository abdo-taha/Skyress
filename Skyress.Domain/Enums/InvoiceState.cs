namespace Skyress.Domain.Enums;

public enum InvoiceState
{
    Draft = 0,
    Issued = 1,
    Paid = 2,
    PartiallyPaid = 3,
    Overdue = 4,
    Cancelled = 5,
}