using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Item
{
    public class Item : AggregateRoot, ISoftDeletable, IAuditable
    {

        public required string Name { get; set; }

        public required string Description { get; set; }

        public double Price { get; set; }

        public double? CostPrice { get; set; }

        public int QuantityLeft { get; set; }

        public int QuantitySold { get; set; }

        public string? QrCode { get; set; }

        public Unit Unit { get; set; }

        public bool IsDeleted { get; set; }

        public string? LastEditBy { get; set; }

        public DateTime LastEditDate { get; set; }

        public DateTime CreatedAt { get; init; }
    }
}
