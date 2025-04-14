namespace Skyress.Domain.Exceptions;

public class DomainException : Exception
{
    protected DomainException(string message)
        : base(message)
    {
    }
}

