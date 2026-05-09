using FluentValidation;

namespace Skyress.Application.Baskets.Commands.CancelBasketReservation;

public class CancelBasketReservationCommandValidator : AbstractValidator<CancelBasketReservationCommand>
{
    public CancelBasketReservationCommandValidator()
    {
        RuleFor(x => x.BasketId).GreaterThan(0);
    }
}
