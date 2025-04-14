using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Payment;

public class Installment : BaseEntity, ISoftDeletable, IAuditable
{
    public long PaymentId { get; set; }

    public decimal DueAmount { get; set; }

    public int number { get; set; }

    public DateTime DueDate { get; set; }

    public PaymentState paymentState { get; set; }

    public string? LastEditBy { get; set; }

    public DateTime LastEditDate { get; set; }

    public DateTime CreaedAt { get; init; }

    public bool IsDeleted { get; set; }
}
