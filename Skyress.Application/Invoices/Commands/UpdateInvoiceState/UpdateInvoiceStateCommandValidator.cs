namespace Skyress.Application.Invoices.Commands.UpdateInvoiceState;

using FluentValidation;

public sealed class UpdateInvoiceStateCommandValidator : AbstractValidator<UpdateInvoiceStateCommand>
{
    public UpdateInvoiceStateCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invoice ID must be valid.");
        RuleFor(x => x.State).IsInEnum().WithMessage("State must be a valid value.");
    }
}
