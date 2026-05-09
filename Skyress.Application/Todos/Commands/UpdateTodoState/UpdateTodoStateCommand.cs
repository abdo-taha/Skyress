using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Todos.Commands.UpdateTodoState;

public record UpdateTodoStateCommand(long Id, TodoState State) : ICommand;

public class UpdateTodoStateCommandHandler : ICommandHandler<UpdateTodoStateCommand>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<UpdateTodoStateCommandHandler> _logger;

    public UpdateTodoStateCommandHandler(ITodoRepository todoRepository, ILogger<UpdateTodoStateCommandHandler> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateTodoStateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateTodoStateCommand));

        var todo = await _todoRepository.GetByIdAsync(request.Id, cancellationToken);

        if (todo is null)
        {
            return Result.Failure(new Error("Not found", ""));
        }

        todo.State = request.State;

        await _todoRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateTodoStateCommand));
        return Result.Success();
    }
}
