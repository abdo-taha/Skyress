namespace Skyress.Domain.Primitives;

using MediatR;

public interface IDomainEvent : INotification
{
    public Guid Id { get; init; }
}
