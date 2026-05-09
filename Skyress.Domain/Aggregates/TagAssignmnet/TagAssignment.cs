using Skyress.Domain.Primitives;

namespace Skyress.Domain.Aggregates.TagAssignment;

public class TagAssignment : AggregateRoot
{
    public long TagId { get; set; }

    public long ItemId { get; set; }
}
