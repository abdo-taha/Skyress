namespace Skyress.Application.Items.Commands.UpdateItemPrice;

using FluentValidation;

public sealed class UpdateItemPriceCommandValidator : AbstractValidator<UpdateItemPriceCommand>
{
    public UpdateItemPriceCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
    }
}
