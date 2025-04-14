namespace Skyress.Domain.primitives;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
}
