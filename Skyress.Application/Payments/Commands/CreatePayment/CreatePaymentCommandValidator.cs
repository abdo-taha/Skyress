namespace Skyress.Application.Payments.Commands.CreatePayment;

using FluentValidation;

public sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0).WithMessage("Invoice ID must be valid.");
    }
}
