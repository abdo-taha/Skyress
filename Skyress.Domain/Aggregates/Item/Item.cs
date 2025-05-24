using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Item
{
    public class Item : AggregateRoot, ISoftDeletable, IAuditable
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public double Price { get; private set; }
        public double? CostPrice { get; private set; }
        public int QuantityLeft { get; private set; }
        public int QuantitySold { get; private set; }
        public string? QrCode { get; private set; }
        public Unit Unit { get; private set; }
        public bool IsDeleted { get; private set; }
        public string? LastEditBy { get; private set; }
        public DateTime LastEditDate { get; private set; }
        public DateTime CreatedAt { get; init; }
        
        private Item() { }

        public static Item Create(
            string name,
            string description,
            double price,
            Unit unit,
            int quantityLeft = 0,
            double? costPrice = null,
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

        public void UpdatePrice(double price, string? editedBy = null)
        {
            Price = price;
            UpdateLastEditDate(editedBy);
        }

        public void UpdateCostPrice(double? costPrice, string? editedBy = null)
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

        public void SoftDelete(string? deletedBy = null)
        {
            IsDeleted = true;
            UpdateLastEditDate(deletedBy);
        }

        public void Restore(string? restoredBy = null)
        {
            IsDeleted = false;
            UpdateLastEditDate(restoredBy);
        }
    }
}
