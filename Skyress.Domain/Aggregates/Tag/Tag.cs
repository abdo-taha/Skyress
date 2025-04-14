using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Tag;

public class Tag : AggregateRoot, ISoftDeletable
{
    public Tag(string name, TagType type)
    {
        Name = name;
        Type = type;
    }

    public bool IsDeleted { get; set; }

    public string Name { get; set; }

    public TagType Type { get; set; }
}
