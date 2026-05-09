namespace Skyress.Application.Invoices.Responses;

using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Enums;

public sealed record InvoiceResponse(
    long Id,
    decimal TotalAmount,
    long BasketId,
    long? CustomerId,
    long? PaymentId,
    InvoiceState State,
    DateTime CreatedAt)
{
    public static InvoiceResponse FromDomain(Invoice invoice) => new(
        invoice.Id,
        invoice.TotalAmount,
        invoice.BasketId,
        invoice.CustomerId,
        invoice.PaymentId,
        invoice.State,
        invoice.CreatedAt);
}
