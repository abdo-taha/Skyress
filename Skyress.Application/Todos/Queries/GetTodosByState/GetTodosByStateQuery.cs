using MediatR;
using Microsoft.Extensions.Logging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Todos.Responses;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Todos.Queries.GetTodosByState;

public record GetTodosByStateQuery(TodoState State) : IRequest<Result<List<TodoResponse>>>;

public class GetTodosByStateQueryHandler : IRequestHandler<GetTodosByStateQuery, Result<List<TodoResponse>>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<GetTodosByStateQueryHandler> _logger;

    public GetTodosByStateQueryHandler(ITodoRepository todoRepository, ILogger<GetTodosByStateQueryHandler> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    public async Task<Result<List<TodoResponse>>> Handle(GetTodosByStateQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetTodosByStateQuery));

        var todos = await _todoRepository.GetTodosByStateAsync(request.State, cancellationToken);
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetTodosByStateQuery), todos.Count);
        return Result.Success(todos.Select(TodoResponse.FromDomain).ToList());
    }
}