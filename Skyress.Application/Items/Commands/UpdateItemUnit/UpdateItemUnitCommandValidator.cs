namespace Skyress.Application.Items.Commands.UpdateItemUnit;

using FluentValidation;
using Skyress.Domain.Enums;

public sealed class UpdateItemUnitCommandValidator : AbstractValidator<UpdateItemUnitCommand>
{
    public UpdateItemUnitCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
        RuleFor(x => x.Unit).IsInEnum().WithMessage("Unit must be a valid value.");
    }
}
