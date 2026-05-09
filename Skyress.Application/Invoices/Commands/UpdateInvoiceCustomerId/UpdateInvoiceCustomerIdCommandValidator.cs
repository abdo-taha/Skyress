namespace Skyress.Application.Invoices.Commands.UpdateInvoiceCustomerId;

using FluentValidation;

public sealed class UpdateInvoiceCustomerIdCommandValidator : AbstractValidator<UpdateInvoiceCustomerIdCommand>
{
    public UpdateInvoiceCustomerIdCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invoice ID must be valid.");
        RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("Customer ID must be valid.");
    }
}
