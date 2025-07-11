using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.ClearBasket;

public class ClearBasketCommandHandler(IBasketRepository basketRepository) : ICommandHandler<ClearBasketCommand>
{
    public async Task<Result> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetByIdAsync(request.BasketId);

        if (basket is null)
        {
            return Result.Failure(new Error("Basket.NotFound", "The basket was not found."));
        }

        basket.Clear();

        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}