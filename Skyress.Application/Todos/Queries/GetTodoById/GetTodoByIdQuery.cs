using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Todos.Responses;
using Skyress.Domain.Common;

namespace Skyress.Application.Todos.Queries.GetTodoById;

public record GetTodoByIdQuery(long Id) : IQuery<TodoResponse>;

public class GetTodoByIdQueryHandler : IQueryHandler<GetTodoByIdQuery, TodoResponse>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<GetTodoByIdQueryHandler> _logger;

    public GetTodoByIdQueryHandler(ITodoRepository todoRepository, ILogger<GetTodoByIdQueryHandler> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    public async Task<Result<TodoResponse>> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetTodoByIdQuery));

        var todo = await _todoRepository.GetByIdAsync(request.Id, cancellationToken);

        if (todo is null)
        {
            return Result<TodoResponse>.Failure(new Error("not found", ""));
        }

        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(GetTodoByIdQuery), todo.Id);
        return Result.Success(TodoResponse.FromDomain(todo));
    }
}