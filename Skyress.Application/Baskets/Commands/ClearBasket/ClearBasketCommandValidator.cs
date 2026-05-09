using FluentValidation;

namespace Skyress.Application.Baskets.Commands.ClearBasket;

public class ClearBasketCommandValidator : AbstractValidator<ClearBasketCommand>
{
    public ClearBasketCommandValidator()
    {
        RuleFor(x => x.BasketId).GreaterThan(0);
    }
}
