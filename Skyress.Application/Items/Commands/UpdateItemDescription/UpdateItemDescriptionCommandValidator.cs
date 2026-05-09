namespace Skyress.Application.Items.Commands.UpdateItemDescription;

using FluentValidation;

public sealed class UpdateItemDescriptionCommandValidator : AbstractValidator<UpdateItemDescriptionCommand>
{
    public UpdateItemDescriptionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
    }
}
