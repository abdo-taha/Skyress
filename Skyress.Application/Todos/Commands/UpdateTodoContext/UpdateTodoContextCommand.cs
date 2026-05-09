using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Todos.Commands.UpdateTodoContext;

public record UpdateTodoContextCommand(long Id, string Context) : ICommand;

public class UpdateTodoContextCommandHandler : ICommandHandler<UpdateTodoContextCommand>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<UpdateTodoContextCommandHandler> _logger;

    public UpdateTodoContextCommandHandler(ITodoRepository todoRepository, ILogger<UpdateTodoContextCommandHandler> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateTodoContextCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateTodoContextCommand));

        var todo = await _todoRepository.GetByIdAsync(request.Id, cancellationToken);

        if (todo is null)
        {
            return Result.Failure(new Error("NOT_FOUND", "Not Found"));
        }

        todo.context = request.Context;

        await _todoRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateTodoContextCommand));
        return Result.Success();
    }
}
