using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Payment;

public class Payment : AggregateRoot, IAuditable, ISoftDeletable
{
    public PaymentType PaymentType { get; set; }

    public PaymentState PaymentState { get; set; }

    public decimal TotalPaid { get; set; }

    public long InvoiceId { get; set; }

    public decimal TotalDue { get; set; }

    public string? LastEditBy { get; set; }

    public DateTime LastEditDate { get; set; }

    public DateTime CreatedAt { get; init; }
    
    public bool IsDeleted { get; private set; }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    public void UnDelete()
    {
        IsDeleted = false;
    }
}
