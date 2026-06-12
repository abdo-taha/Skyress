using FluentValidation;

namespace Skyress.Application.Baskets.Commands.CompleteCheckout;

public sealed class CompleteCheckoutCommandValidator : AbstractValidator<CompleteCheckoutCommand>
{
    public CompleteCheckoutCommandValidator()
    {
        RuleFor(x => x.BasketId)
            .GreaterThan(0)
            .WithMessage("Basket ID must be valid.");
    }
}
