namespace Skyress.Application.Tags.Commands.UpdateTagType;

using FluentValidation;

public sealed class UpdateTagTypeCommandValidator : AbstractValidator<UpdateTagTypeCommand>
{
    public UpdateTagTypeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Tag ID must be valid.");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Type must be a valid value.");
    }
}
