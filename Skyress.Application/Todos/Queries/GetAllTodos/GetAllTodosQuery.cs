using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Todos.Responses;
using Skyress.Domain.Common;

namespace Skyress.Application.Todos.Queries.GetAllTodos;

public record GetAllTodosQuery : IQuery<List<TodoResponse>>;

public class GetAllTodosQueryHandler : IQueryHandler<GetAllTodosQuery, List<TodoResponse>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<GetAllTodosQueryHandler> _logger;

    public GetAllTodosQueryHandler(ITodoRepository todoRepository, ILogger<GetAllTodosQueryHandler> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    public async Task<Result<List<TodoResponse>>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetAllTodosQuery));

        var todos = await _todoRepository.GetAllAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetAllTodosQuery), todos.Count);
        return Result.Success(todos.Select(TodoResponse.FromDomain).ToList());
    }
}