namespace Skyress.Application.Tags.Commands.CreateTag;

using FluentValidation;

public sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Type must be a valid value.");
    }
}
