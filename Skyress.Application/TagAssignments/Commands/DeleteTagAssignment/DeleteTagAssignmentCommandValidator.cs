namespace Skyress.Application.TagAssignments.Commands.DeleteTagAssignment;

using FluentValidation;

public sealed class DeleteTagAssignmentCommandValidator : AbstractValidator<DeleteTagAssignmentCommand>
{
    public DeleteTagAssignmentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("TagAssignment ID must be valid.");
    }
}
