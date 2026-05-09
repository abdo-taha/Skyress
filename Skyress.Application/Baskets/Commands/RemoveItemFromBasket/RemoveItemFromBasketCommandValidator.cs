using FluentValidation;

namespace Skyress.Application.Baskets.Commands.RemoveItemFromBasket;

public class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
{
    public RemoveItemFromBasketCommandValidator()
    {
        RuleFor(x => x.BasketId).GreaterThan(0);
        RuleFor(x => x.ItemId).GreaterThan(0);
    }
}
