namespace Skyress.Application.Items.Commands.UpdateItemName;

using FluentValidation;

public sealed class UpdateItemNameCommandValidator : AbstractValidator<UpdateItemNameCommand>
{
    public UpdateItemNameCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }
}
