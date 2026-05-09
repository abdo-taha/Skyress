namespace Skyress.Application.Payments.Commands.CompleteCashPayment;

using FluentValidation;

public sealed class CompleteCashPaymentCommandValidator : AbstractValidator<CompleteCashPaymentCommand>
{
    public CompleteCashPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).GreaterThan(0).WithMessage("Payment ID must be valid.");
        RuleFor(x => x.TotalPaid).GreaterThan(0).WithMessage("Amount paid must be greater than zero.");
    }
}
