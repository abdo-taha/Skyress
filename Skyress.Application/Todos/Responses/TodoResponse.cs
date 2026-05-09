namespace Skyress.Application.Todos.Responses;

using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Enums;

public sealed record TodoResponse(
    long Id,
    string Context,
    TodoState State,
    DateTime CreatedAt)
{
    public static TodoResponse FromDomain(Todo todo) => new(
        todo.Id,
        todo.context,
        todo.State,
        todo.CreatedAt);
}
