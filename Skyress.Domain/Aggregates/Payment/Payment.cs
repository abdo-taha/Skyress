using Skyress.Domain.Enums;
using Skyress.Domain.Exceptions;
using Skyress.Domain.Primitives;

namespace Skyress.Domain.Aggregates.Payment;

public class Payment : AggregateRoot, IAuditable, ISoftDeletable
{
    public PaymentType PaymentType { get; private set; }
    public PaymentState PaymentState { get; private set; }
    public decimal TotalPaid { get; private set; }
    public long InvoiceId { get; private set; }
    public decimal TotalDue { get; private set; }
    public string? LastEditBy { get; set; }
    public DateTime LastEditDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; private set; }

    public static Payment Create(long invoiceId, decimal totalDue, PaymentType paymentType)
    {
        if (totalDue < 0)
        {
            throw new PaymentInvalidAmountException("Payment total due cannot be negative.");
        }

        return new Payment
        {
            InvoiceId = invoiceId,
            TotalPaid = 0,
            TotalDue = totalDue,
            PaymentType = paymentType,
            PaymentState = PaymentState.Initiated
        };
    }

    public void CompleteCashPayment(decimal totalPaid)
    {
        if (PaymentType != PaymentType.Cash)
        {
            throw new PaymentInvalidStateException("Only cash payments can be completed as cash payments.");
        }

        if (PaymentState != PaymentState.Initiated)
        {
            throw new PaymentInvalidStateException("Only initiated payments can be completed.");
        }

        if (TotalDue != totalPaid)
        {
            throw new PaymentInvalidAmountException("Paid amount must equal total due.");
        }

        PaymentState = PaymentState.Paid;
        TotalPaid = TotalDue;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    public void UnDelete()
    {
        IsDeleted = false;
    }
}
