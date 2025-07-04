using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Invoice
{
    public class Invoice : AggregateRoot, ISoftDeletable, IAuditable
    {
        public decimal TotalAmount { get; set; }

        public long? CustomerId { get; set; }

        public string? LastEditBy { get; set; }

        public DateTime LastEditDate { get; set; }

        public DateTime CreatedAt { get; set; }

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
}
