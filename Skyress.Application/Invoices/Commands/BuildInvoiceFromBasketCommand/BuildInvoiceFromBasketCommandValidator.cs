namespace Skyress.Application.Invoices.Commands.BuildInvoiceFromBasketCommand;

using FluentValidation;

public sealed class BuildInvoiceFromBasketCommandValidator : AbstractValidator<BuildInvoiceFromBasketCommand>
{
    public BuildInvoiceFromBasketCommandValidator()
    {
        RuleFor(x => x.BasketId).GreaterThan(0).WithMessage("Basket ID must be valid.");
    }
}
