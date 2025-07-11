namespace Skyress.Domain.Common;

public class Error(string code, string message) : IEquatable<Error>
{
    public static readonly Error None = new Error(string.Empty, string.Empty);
    
    public static readonly Error Dummy = new Error(string.Empty, string.Empty);

    public string Code { get; } = code;
    public string Message { get; } = message;

    public static implicit operator string(Error error) => error.Code;

    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Code == b.Code;
    }
    public static bool operator !=(Error? a, Error? b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != GetType()) return false;
        if (obj is not Error error) return false;
        return error.Code == Code;
    }

    public bool Equals(Error? other)
    {
        if (other == null) return false;
        if (other.GetType() != GetType()) return false;
        return other.Code == Code;
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
}
