namespace Skyress.Domain.primitives;

using MediatR;

public interface IDomainEvent : INotification
{
    public Guid Id { get; init; }
}
