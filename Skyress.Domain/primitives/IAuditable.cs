namespace Skyress.Domain.Primitives;

public interface IAuditable
{
    public string? LastEditBy { get; set; }

    public DateTime LastEditDate { get; set; }

    public DateTime CreatedAt { get; set; }
}
