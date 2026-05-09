namespace Skyress.Application.Todos.Commands.UpdateTodoState;

using FluentValidation;

public sealed class UpdateTodoStateCommandValidator : AbstractValidator<UpdateTodoStateCommand>
{
    public UpdateTodoStateCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Todo ID must be valid.");
        RuleFor(x => x.State).IsInEnum().WithMessage("State must be a valid value.");
    }
}
