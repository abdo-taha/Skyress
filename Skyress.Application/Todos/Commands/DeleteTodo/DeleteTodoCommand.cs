using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Todos.Commands.DeleteTodo;

public record DeleteTodoCommand(long Id) : ICommand;

public class DeleteTodoCommandHandler : ICommandHandler<DeleteTodoCommand>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<DeleteTodoCommandHandler> _logger;

    public DeleteTodoCommandHandler(ITodoRepository todoRepository, ILogger<DeleteTodoCommandHandler> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command} for TodoId: {Id}", nameof(DeleteTodoCommand), request.Id);

        await _todoRepository.DeleteByIdAsync(request.Id, cancellationToken);
        await _todoRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. TodoId: {Id}", nameof(DeleteTodoCommand), request.Id);
        return Result.Success();
    }
}
