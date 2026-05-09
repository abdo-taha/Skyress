using FluentValidation;

namespace Skyress.Application.Baskets.Commands.DeleteBasketCommand;

public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
{
    public DeleteBasketCommandValidator()
    {
        RuleFor(x => x.BasketId).GreaterThan(0);
    }
}
