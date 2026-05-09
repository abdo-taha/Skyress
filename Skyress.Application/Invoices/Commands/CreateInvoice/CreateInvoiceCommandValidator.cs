namespace Skyress.Application.Invoices.Commands.CreateInvoice;

using FluentValidation;

public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.BasketId).GreaterThan(0).WithMessage("Basket ID must be valid.");
    }
}
