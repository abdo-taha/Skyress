using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Invoice
{
    public class SoldItem : BaseEntity, ISoftDeletable, IAuditable
    {

        public required string Name { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public TransactionType TransactionType { get; set; }

        public DateTime SellingTime { get; set; }

        public long InvoiceId { get; set; }

        public long? ItemId { get; set; }

        public double ItemCost { get; set; }

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
}
