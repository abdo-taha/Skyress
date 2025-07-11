using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.RemoveItemFromBasket;

public class RemoveItemFromBasketCommandHandler(IBasketRepository basketRepository) : ICommandHandler<RemoveItemFromBasketCommand>
{
    public async Task<Result> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetByIdAsync(request.BasketId);

        if (basket is null)
        {
            return Result.Failure(new Error("Basket.NotFound", "The basket was not found."));
        }

        var result = basket.RemoveItem(request.ItemId);

        if (result.IsFailure)
        {
            return result;
        }

        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}