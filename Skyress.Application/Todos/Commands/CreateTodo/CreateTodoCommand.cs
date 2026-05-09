using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Todos.Responses;
using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Todos.Commands.CreateTodo;

public record CreateTodoCommand(string Context) : ICommand<TodoResponse>;

public class CreateTodoCommandHandler : ICommandHandler<CreateTodoCommand, TodoResponse>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<CreateTodoCommandHandler> _logger;

    public CreateTodoCommandHandler(ITodoRepository todoRepository, ILogger<CreateTodoCommandHandler> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    public async Task<Result<TodoResponse>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(CreateTodoCommand));

        var todo = new Todo
        {
            context = request.Context,
            State = TodoState.NotAcknowledge,
            CreatedAt = DateTime.UtcNow
        };

        await _todoRepository.CreateAsync(todo, cancellationToken);
        await _todoRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(CreateTodoCommand), todo.Id);
        return Result.Success(TodoResponse.FromDomain(todo));
    }
}
