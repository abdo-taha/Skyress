namespace Skyress.Application.Tags.Commands.UpdateTagName;

using FluentValidation;

public sealed class UpdateTagNameCommandValidator : AbstractValidator<UpdateTagNameCommand>
{
    public UpdateTagNameCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Tag ID must be valid.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }
}
