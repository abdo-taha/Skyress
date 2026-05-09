namespace Skyress.Application.Invoices.Commands.DeleteInvoice;

using FluentValidation;

public sealed class DeleteInvoiceCommandValidator : AbstractValidator<DeleteInvoiceCommand>
{
    public DeleteInvoiceCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invoice ID must be valid.");
    }
}
