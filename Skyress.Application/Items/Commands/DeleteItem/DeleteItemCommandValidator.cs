namespace Skyress.Application.Items.Commands.DeleteItem;

using FluentValidation;

public sealed class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
    }
}
