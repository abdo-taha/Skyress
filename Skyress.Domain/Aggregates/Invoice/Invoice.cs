using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Invoice
{
    public class Invoice : AggregateRoot, ISoftDeletable, IAuditable
    {
        public double TotalAmount { get; set; }

        public long? CustomerId { get; set; }

        public string? LastEditBy { get; set; }

        public DateTime LastEditDate { get; set; }

        public DateTime CreaedAt { get; init; }

        public bool IsDeleted { get; set; }
    }
}
