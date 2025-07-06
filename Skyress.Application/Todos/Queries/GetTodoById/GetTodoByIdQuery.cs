using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Common;

namespace Skyress.Application.Todos.Queries.GetTodoById;

public record GetTodoByIdQuery(long Id) : IQuery<Todo>;

public class GetTodoByIdQueryHandler : IQueryHandler<GetTodoByIdQuery, Todo>
{
    private readonly ITodoRepository _todoRepository;

    public GetTodoByIdQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<Todo>> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdAsync(request.Id);

        if (todo is null)
        {
            return Result<Todo>.Failure(new Error("not found",""));
        }

        return Result.Success(todo);
    }
}