using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

namespace Skyress.Application.Items.Commands.MarkItemsAsSold;

public record MarkItemsAsSoldCommand(long BasketId) : ICommand;

public class MarkItemsAsSoldCommandHandler : ICommandHandler<MarkItemsAsSoldCommand>
{
    private readonly IItemRepository _itemRepository;
    private readonly IBasketRepository _basketRepository;

    public MarkItemsAsSoldCommandHandler(IItemRepository itemRepository, IBasketRepository basketRepository)
    {
        _itemRepository = itemRepository;
        _basketRepository = basketRepository;
    }

    public async Task<Result> Handle(MarkItemsAsSoldCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketWithItemsAsync(request.BasketId);
        if (basket == null)
        {
            return Result.Failure(Error.Dummy);
        }
        
        var itemIds = basket.BasketItems.Select(i => i.ItemId).ToList();
        var items = (await _itemRepository.GetByIdsAsync(itemIds)).ToDictionary(item => item.Id);

        foreach (var soldItem in basket.BasketItems)
        {
            if (!items.TryGetValue(soldItem.ItemId, out var item))
            {
                return Result.Failure(new Error("Item.NotFound", $"Item with ID {soldItem.ItemId} not found"));
            }


            Result result = item.MarkAsSold(soldItem.Quantity);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
        }

        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
} 