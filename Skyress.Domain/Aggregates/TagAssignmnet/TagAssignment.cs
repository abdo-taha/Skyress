using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.TagAssignmnet;

public class TagAssignment : AggregateRoot
{
    public long TagId { get; set; }

    public long ItemId { get; set; }
}
