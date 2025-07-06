using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Todos.Commands.DeleteTodo;

public record DeleteTodoCommand(long Id) : ICommand;

public class DeleteTodoCommandHandler : ICommandHandler<DeleteTodoCommand>
{
    private readonly ITodoRepository _todoRepository;

    public DeleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        await _todoRepository.DeleteByIdAsync(request.Id);
        await _todoRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}