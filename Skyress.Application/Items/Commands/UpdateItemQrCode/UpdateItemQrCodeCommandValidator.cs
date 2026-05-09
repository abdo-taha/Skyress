namespace Skyress.Application.Items.Commands.UpdateItemQrCode;

using FluentValidation;

public sealed class UpdateItemQrCodeCommandValidator : AbstractValidator<UpdateItemQrCodeCommand>
{
    public UpdateItemQrCodeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
    }
}
