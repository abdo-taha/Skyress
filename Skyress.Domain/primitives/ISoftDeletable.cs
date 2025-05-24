namespace Skyress.Domain.primitives;

public interface ISoftDeletable
{
    public bool IsDeleted { get;}

    public void SoftDelete();

    public void UnDelete();
}
