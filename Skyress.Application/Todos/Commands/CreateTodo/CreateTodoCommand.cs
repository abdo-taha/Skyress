using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Todos.Commands.CreateTodo;

public record CreateTodoCommand(string Context) : ICommand<Todo>;

public class CreateTodoCommandHandler : ICommandHandler<CreateTodoCommand, Todo>
{
    private readonly ITodoRepository _todoRepository;

    public CreateTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Result<Todo>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new Todo
        {
            context = request.Context,
            State = TodoState.NotAcknowledge,
            CreatedAt = DateTime.UtcNow
        };

        await _todoRepository.CreateAsync(todo);
        await _todoRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(todo);
    }
}