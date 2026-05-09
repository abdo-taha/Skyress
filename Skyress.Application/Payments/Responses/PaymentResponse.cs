namespace Skyress.Application.Payments.Responses;

using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Enums;

public sealed record PaymentResponse(
    long Id,
    PaymentType PaymentType,
    PaymentState PaymentState,
    decimal TotalPaid,
    decimal TotalDue,
    long InvoiceId,
    DateTime CreatedAt)
{
    public static PaymentResponse FromDomain(Payment payment) => new(
        payment.Id,
        payment.PaymentType,
        payment.PaymentState,
        payment.TotalPaid,
        payment.TotalDue,
        payment.InvoiceId,
        payment.CreatedAt);
}
