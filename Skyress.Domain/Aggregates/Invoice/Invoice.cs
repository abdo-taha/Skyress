using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Invoice
{
    public class Invoice : AggregateRoot, ISoftDeletable, IAuditable
    {
        public decimal TotalAmount { get; set; }

        public long? CustomerId { get; set; }
        
        public InvoiceState State { get; set; } = InvoiceState.Draft;

        public string? LastEditBy { get; set; }

        public DateTime LastEditDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; private set; }

        private readonly List<SoldItem> _soldItems = new();
        public IReadOnlyCollection<SoldItem> SoldItems => _soldItems.AsReadOnly();

        public void AddSoldItem(SoldItem soldItem)
        {
            _soldItems.Add(soldItem);
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
}
