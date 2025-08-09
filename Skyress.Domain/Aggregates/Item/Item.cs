using Skyress.Domain.Enums;
using Skyress.Domain.primitives;
using Skyress.Domain.Aggregates.Item.Events;
using Skyress.Domain.Common;

namespace Skyress.Domain.Aggregates.Item
{
    public class Item : AggregateRoot, ISoftDeletable, IAuditable
    {
        private readonly List<PricingHistory> _pricingHistory = new();
        public IReadOnlyCollection<PricingHistory> PricingHistory => _pricingHistory.AsReadOnly();

        public string Name { get; private set; }
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public decimal? CostPrice { get; private set; }
        public int QuantityLeft { get; private set; }
        public int QuantityReserved { get; private set; }
        public int QuantitySold { get; private set; }
        public string? QrCode { get; private set; }
        public Unit Unit { get; private set; }
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

        private Item(string name)
        {
            Name = name;
        }
        
        public static Item Create(
            string name,
            string description,
            decimal price,
            Unit unit,
            int quantityLeft = 0,
            decimal? costPrice = null,
            string? qrCode = null)
        {
            var item = new Item(name)
            {
                Description = description,
                Price = price,
                CostPrice = costPrice,
                QuantityLeft = quantityLeft,
                QuantityReserved = 0,
                QuantitySold = 0,
                QrCode = qrCode,
                Unit = unit,
                IsDeleted = false
            };
            return item;
        }

        public void UpdateName(string name)
        {
            Name = name;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }

        public void UpdatePrice(decimal newPrice,PricingChangeType pricingChangeType = PricingChangeType.PriceChange)
        {
            decimal oldPrice = Price;
            Price = newPrice;
            RaiseDomainEvent(new ItemPriceChangedDomainEvent(Guid.NewGuid(), Id, oldPrice, newPrice, CostPrice ?? 0M, CostPrice ?? 0M, pricingChangeType));
        }

        public void AddPricingHistory(PricingHistory pricingHistory)
        {
            this._pricingHistory.Add(pricingHistory);
        }

        public void UpdateCostPrice(decimal? costPrice)
        {
            CostPrice = costPrice;
        }

        public void UpdateQuantityLeft(int quantityLeft)
        {
            QuantityLeft = quantityLeft;
        }

        public void UpdateQrCode(string? qrCode)
        {
            QrCode = qrCode;
        }

        public void UpdateUnit(Unit unit)
        {
            Unit = unit;
        }

        
        public Result ReserveQuantity(int quantity)
        {
            if (QuantityLeft - QuantityReserved < quantity)
            {
                return Result.Failure(new Error("Item.InsufficientStock", 
                    $"Insufficient available stock. Available: {QuantityLeft - QuantityReserved}, Requested: {quantity}"));
            }

            QuantityReserved += quantity;
            return Result.Success();
        }

        public Result ReleaseReservation(int quantity)
        {
            if (QuantityReserved < quantity)
            {
                return Result.Failure(new Error("Item.InvalidReservation", 
                    $"Cannot release more than reserved. Reserved: {QuantityReserved}, Requested: {quantity}"));
            }

            QuantityReserved -= quantity;
            return Result.Success();
        }

        public void MarkAsSold(int quantity)
        {
            QuantityReserved -= quantity;
            QuantityLeft -= quantity;
            QuantitySold += quantity;
        }
    }
}
