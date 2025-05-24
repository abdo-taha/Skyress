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
    
    public string Name { get; set; }

    public TagType Type { get; set; }
    
    public bool IsDeleted { get; private set; }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    public void UnDelete()
    {
        IsDeleted = false;
    }
}
