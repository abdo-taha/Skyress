using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.AddItemToBasket;

public record AddItemToBasketCommand(long BasketId, long ItemId, int Quantity) : ICommand<Basket>;

public class AddItemToBasketCommandHandler(IBasketRepository basketRepository, IItemRepository itemRepository)
    : ICommandHandler<AddItemToBasketCommand, Basket>
{
    public async Task<Result<Basket>> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetByIdAsync(request.BasketId);
        
        if (basket is null)
        {
            return Result<Basket>.Failure(Error.Dummy);
        }

        var item = await itemRepository.GetByIdAsync(request.ItemId);
        if (item is null)
        {
            return Result<Basket>.Failure(Error.Dummy);
        }

        var result = basket.AddItem(item.Id, request.Quantity);
        if (result.IsFailure)
        {
            return Result<Basket>.Failure(result.Error);
        }
        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(basket);
    }
}