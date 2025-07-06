using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Todos.Commands.UpdateTodoState;

public record UpdateTodoStateCommand(long Id, TodoState State) : ICommand;

public class UpdateTodoStateCommandHandler : ICommandHandler<UpdateTodoStateCommand>
{
    private readonly ITodoRepository _todoRepository;

    public UpdateTodoStateCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result> Handle(UpdateTodoStateCommand request, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdAsync(request.Id);

        if (todo is null)
        {
            return Result.Failure(new Error("Not found", ""));
        }

        todo.State = request.State;

        await _todoRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}