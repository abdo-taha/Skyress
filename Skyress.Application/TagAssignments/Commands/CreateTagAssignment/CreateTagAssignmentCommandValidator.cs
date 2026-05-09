namespace Skyress.Application.TagAssignments.Commands.CreateTagAssignment;

using FluentValidation;

public sealed class CreateTagAssignmentCommandValidator : AbstractValidator<CreateTagAssignmentCommand>
{
    public CreateTagAssignmentCommandValidator()
    {
        RuleFor(x => x.TagId).GreaterThan(0).WithMessage("Tag ID must be valid.");
        RuleFor(x => x.ItemId).GreaterThan(0).WithMessage("Item ID must be valid.");
    }
}
