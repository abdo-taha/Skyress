using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

namespace Skyress.Application.Items.Commands.MarkItemsAsSold;

public record MarkItemsAsSoldCommand(Dictionary<long, int> ItemQuantities) : ICommand;

public class MarkItemsAsSoldCommandHandler : ICommandHandler<MarkItemsAsSoldCommand>
{
    private readonly IItemRepository _itemRepository;

    public MarkItemsAsSoldCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Result> Handle(MarkItemsAsSoldCommand request, CancellationToken cancellationToken)
    {
        var itemIds = request.ItemQuantities.Keys.ToList();
        var items = (await _itemRepository.GetByIdsAsync(itemIds)).ToDictionary(item => item.Id);

        foreach (var kvp in request.ItemQuantities)
        {
            if (!items.TryGetValue(kvp.Key, out var item))
            {
                return Result.Failure(new Error("Item.NotFound", $"Item with ID {kvp.Key} not found"));
            }

            if (item.QuantityLeft < kvp.Value)
            {
                return Result.Failure(new Error("Item.InsufficientStock", 
                    $"Insufficient stock for item {item.Name}. Available: {item.QuantityLeft}, Requested: {kvp.Value}"));
            }

            item.MarkAsSold(kvp.Value);
        }

        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
} 