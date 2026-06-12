using FluentValidation;

namespace Skyress.Application.Baskets.Commands.ReserveItems;

public sealed class ReserveItemsCommandValidator : AbstractValidator<ReserveItemsCommand>
{
    public ReserveItemsCommandValidator()
    {
        RuleFor(x => x.BasketId)
            .GreaterThan(0)
            .WithMessage("Basket ID must be valid.");
    }
}
