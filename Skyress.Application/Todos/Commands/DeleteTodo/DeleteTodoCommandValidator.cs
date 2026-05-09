namespace Skyress.Application.Todos.Commands.DeleteTodo;

using FluentValidation;

public sealed class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Todo ID must be valid.");
    }
}
