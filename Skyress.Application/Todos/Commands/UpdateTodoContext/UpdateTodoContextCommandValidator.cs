namespace Skyress.Application.Todos.Commands.UpdateTodoContext;

using FluentValidation;

public sealed class UpdateTodoContextCommandValidator : AbstractValidator<UpdateTodoContextCommand>
{
    public UpdateTodoContextCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Todo ID must be valid.");
        RuleFor(x => x.Context).NotEmpty().WithMessage("Context is required.");
    }
}
