using MediatR;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Todos.Queries.GetTodosByState;

public record GetTodosByStateQuery(TodoState State) : IRequest<Result<List<Todo>>>;

public class GetTodosByStateQueryHandler : IRequestHandler<GetTodosByStateQuery, Result<List<Todo>>>
{
    private readonly ITodoRepository _todoRepository;

    public GetTodosByStateQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<List<Todo>>> Handle(GetTodosByStateQuery request, CancellationToken cancellationToken)
    {
        var todos = await _todoRepository.GetTodosByStateAsync(request.State, cancellationToken);
        return Result.Success(todos);
    }
}