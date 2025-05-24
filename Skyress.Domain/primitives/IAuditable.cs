namespace Skyress.Domain.primitives;

public interface IAuditable
{
    public string? LastEditBy { get;}

    public DateTime LastEditDate { get;}

    public DateTime CreatedAt { get;}
}
