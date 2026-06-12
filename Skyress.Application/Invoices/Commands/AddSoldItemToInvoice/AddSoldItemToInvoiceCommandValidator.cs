namespace Skyress.Application.Invoices.Commands.AddSoldItemToInvoice;

using FluentValidation;

public sealed class AddSoldItemToInvoiceCommandValidator : AbstractValidator<AddSoldItemToInvoiceCommand>
{
    public AddSoldItemToInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0).WithMessage("Invoice ID must be valid.");
        RuleFor(x => x.ItemId).GreaterThan(0).WithMessage("Item ID must be valid.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
