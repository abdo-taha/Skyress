namespace Skyress.Application.Todos.Commands.CreateTodo;

using FluentValidation;

public sealed class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Context).NotEmpty().WithMessage("Context is required.");
    }
}
