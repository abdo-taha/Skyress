using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Todos.Commands.UpdateTodoContext;

public record UpdateTodoContextCommand(long Id, string Context) : ICommand;

public class UpdateTodoContextCommandHandler : ICommandHandler<UpdateTodoContextCommand>
{
    private readonly ITodoRepository _todoRepository;

    public UpdateTodoContextCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result> Handle(UpdateTodoContextCommand request, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdAsync(request.Id);

        if (todo is null)
        {
            return Result.Failure(new Error("NOT_FOUND", "Not Found"));
        }

        todo.context = request.Context;

        await _todoRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}