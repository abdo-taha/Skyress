using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Todo;

public class Todo : AggregateRoot, IAuditable, ISoftDeletable
{
    public TodoState State { get; set; }

    public required string context { get; set; }

    public string? LastEditBy { get; set; }

    public DateTime LastEditDate { get; set; }

    public DateTime CreatedAt { get; init; }

    public bool IsDeleted { get; set; }
}
