using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.AddItemToBasket;

public record AddItemToBasketCommand(long BasketId, long ItemId, int Quantity) : ICommand<Basket>;

public class AddItemToBasketCommandHandler(IBasketRepository basketRepository, IItemRepository itemRepository, ILogger<AddItemToBasketCommandHandler> logger)
    : ICommandHandler<AddItemToBasketCommand, Basket>
{
    private readonly ILogger<AddItemToBasketCommandHandler> _logger = logger;

    public async Task<Result<Basket>> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(AddItemToBasketCommand));

        var basket = await basketRepository.GetBasketWithItemsAsync(request.BasketId);

        if (basket is null)
        {
            return Result<Basket>.Failure(Error.Dummy);
        }

        var item = await itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
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
        _logger.LogInformation("{Command} completed", nameof(AddItemToBasketCommand));
        return Result.Success(basket);
    }
}