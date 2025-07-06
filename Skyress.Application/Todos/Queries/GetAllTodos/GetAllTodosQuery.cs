using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Common;

namespace Skyress.Application.Todos.Queries.GetAllTodos;

public record GetAllTodosQuery : IQuery<List<Todo>>;

public class GetAllTodosQueryHandler : IQueryHandler<GetAllTodosQuery, List<Todo>>
{
    private readonly ITodoRepository _todoRepository;

    public GetAllTodosQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<List<Todo>>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        var todos = await _todoRepository.GetAllAsync();
        return Result.Success(todos.ToList());
    }
}