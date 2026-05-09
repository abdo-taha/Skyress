using FluentValidation;

namespace Skyress.Application.Baskets.Commands.AddItemToBasket;

public class AddItemToBasketCommandValidator : AbstractValidator<AddItemToBasketCommand>
{
    public AddItemToBasketCommandValidator()
    {
        RuleFor(x => x.BasketId).GreaterThan(0);
        RuleFor(x => x.ItemId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
