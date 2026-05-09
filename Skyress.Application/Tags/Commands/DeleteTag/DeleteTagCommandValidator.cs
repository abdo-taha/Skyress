namespace Skyress.Application.Tags.Commands.DeleteTag;

using FluentValidation;

public sealed class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Tag ID must be valid.");
    }
}
