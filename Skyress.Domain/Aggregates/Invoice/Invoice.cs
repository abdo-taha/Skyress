using Skyress.Domain.Enums;
using Skyress.Domain.Primitives;
using Skyress.Domain.Exceptions;

namespace Skyress.Domain.Aggregates.Invoice
{
    public class Invoice : AggregateRoot, ISoftDeletable, IAuditable
    {
        public decimal TotalAmount { get; private set; }
        
        public long BasketId { get; set; }

        public long? CustomerId { get; set; }

        public long? PaymentId { get; private set; }
        
        public InvoiceState State { get; private set; } = InvoiceState.Draft;

        public string? LastEditBy { get; set; }

        public DateTime LastEditDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; private set; }

        private readonly List<SoldItem> _soldItems = new();
        public IReadOnlyCollection<SoldItem> SoldItems => _soldItems.AsReadOnly();

        public static Invoice Create(long basketId, long? customerId, InvoiceState state = InvoiceState.Draft)
        {
            var invoice = new Invoice
            {
                BasketId = basketId,
                CustomerId = customerId
            };

            invoice.UpdateState(state);
            return invoice;
        }

        public void AddSoldItem(SoldItem soldItem)
        {
            if (soldItem.ItemId is not null && _soldItems.Any(si => si.ItemId == soldItem.ItemId))
                throw new InvoiceDuplicateSoldItemException(soldItem.ItemId.Value);

            _soldItems.Add(soldItem);
            TotalAmount += soldItem.Price * soldItem.Quantity;
        }

        public void Issue()
        {
            if (State >= InvoiceState.Issued)
                return;

            if (State != InvoiceState.Draft)
                throw new InvoiceInvalidStateException("Only draft invoices can be issued.");

            State = InvoiceState.Issued;
        }

        public void AttachPayment(long paymentId)
        {
            if (PaymentId is not null && PaymentId != paymentId)
                throw new InvoicePaymentAlreadyAttachedException();

            PaymentId = paymentId;
        }

        public void UpdateState(InvoiceState state)
        {
            if (State == state)
                return;

            if (State == InvoiceState.Cancelled)
                throw new InvoiceInvalidStateException("Cancelled invoices cannot change state.");

            State = state;
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
