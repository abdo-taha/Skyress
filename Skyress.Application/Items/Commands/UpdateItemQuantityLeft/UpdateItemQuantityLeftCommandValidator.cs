namespace Skyress.Application.Items.Commands.UpdateItemQuantityLeft;

using FluentValidation;

public sealed class UpdateItemQuantityLeftCommandValidator : AbstractValidator<UpdateItemQuantityLeftCommand>
{
    public UpdateItemQuantityLeftCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
        RuleFor(x => x.QuantityLeft).GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative.");
    }
}
