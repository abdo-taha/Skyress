namespace Skyress.Application.Invoices.Commands.AddSoldItemToInvoice;

using FluentValidation;

public sealed class AddSoldItemToInvoiceCommandValidator : AbstractValidator<AddSoldItemToInvoiceCommand>
{
    public AddSoldItemToInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0).WithMessage("Invoice ID must be valid.");
    }
}
