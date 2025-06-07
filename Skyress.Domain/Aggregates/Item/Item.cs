using Skyress.Domain.Enums;
using Skyress.Domain.primitives;
using Skyress.Domain.Aggregates.Item.Events;

namespace Skyress.Domain.Aggregates.Item
{
    public class Item : AggregateRoot, ISoftDeletable, IAuditable
    {
        private readonly List<PricingHistory> _pricingHistory = new();
        public IReadOnlyCollection<PricingHistory> PricingHistory => _pricingHistory.AsReadOnly();

        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public decimal? CostPrice { get; private set; }
        public int QuantityLeft { get; private set; }
        public int QuantitySold { get; private set; }
        public string? QrCode { get; private set; }
        public Unit Unit { get; private set; }
        public string? LastEditBy { get; private set; }
        public DateTime LastEditDate { get; private set; }
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
        
        private Item() { }

        public static Item Create(
            string name,
            string description,
            decimal price,
            Unit unit,
            int quantityLeft = 0,
            decimal? costPrice = null,
            string? qrCode = null,
            string? createdBy = null)
        {
            var item = new Item
            {
                Name = name,
                Description = description,
                Price = price,
                CostPrice = costPrice,
                QuantityLeft = quantityLeft,
                QuantitySold = 0,
                QrCode = qrCode,
                Unit = unit,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                LastEditBy = createdBy,
            };
            item.UpdateLastEditDate();
            return item;
        }

        private void UpdateLastEditDate(string? editedBy = null)
        {
            LastEditDate = DateTime.UtcNow;
            if (editedBy != null)
            {
                LastEditBy = editedBy;
            }
        }

        public void UpdateName(string name, string? editedBy = null)
        {
            Name = name;
            UpdateLastEditDate(editedBy);
        }

        public void UpdateDescription(string description, string? editedBy = null)
        {
            Description = description;
            UpdateLastEditDate(editedBy);
        }

        public void UpdatePrice(decimal newPrice,PricingChangeType pricingChangeType = PricingChangeType.PriceChange, string? editedBy = null)
        {
            decimal oldPrice = Price;
            Price = newPrice;
            RaiseDomainEvent(new ItemPriceChangedDomainEvent(Guid.NewGuid(), Id, oldPrice, newPrice, CostPrice ?? 0M, CostPrice ?? 0M, pricingChangeType, editedBy, DateTime.UtcNow, DateTime.UtcNow));
            UpdateLastEditDate(editedBy);
        }

        public void AddPricingHistory(PricingHistory pricingHistory)
        {
            this._pricingHistory.Add(pricingHistory);
        }

        public void UpdateCostPrice(decimal? costPrice, string? editedBy = null)
        {
            CostPrice = costPrice;
            UpdateLastEditDate(editedBy);
        }

        public void UpdateQuantityLeft(int quantityLeft, string? editedBy = null)
        {
            QuantityLeft = quantityLeft;
            UpdateLastEditDate(editedBy);
        }

        public void UpdateQrCode(string? qrCode, string? editedBy = null)
        {
            QrCode = qrCode;
            UpdateLastEditDate(editedBy);
        }

        public void UpdateUnit(Unit unit, string? editedBy = null)
        {
            Unit = unit;
            UpdateLastEditDate(editedBy);
        }

        public void MarkAsSold(int quantity)
        {
            if (QuantityLeft < quantity)
            {
                return; 
            }
            QuantityLeft -= quantity;
            QuantitySold += quantity;
            UpdateLastEditDate();
        }
    }
}
