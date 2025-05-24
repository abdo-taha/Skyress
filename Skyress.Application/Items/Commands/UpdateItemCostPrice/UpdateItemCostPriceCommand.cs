namespace Skyress.Application.Items.Commands.UpdateItemCostPrice;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

public record UpdateItemCostPriceCommand(
    long Id,
    double? CostPrice) : ICommand<Item>;

public class UpdateItemCostPriceCommandHandler : ICommandHandler<UpdateItemCostPriceCommand, Item>
{
    private readonly IItemRepository _itemRepository;

    public UpdateItemCostPriceCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Result<Item>> Handle(UpdateItemCostPriceCommand request, CancellationToken cancellationToken)
    {
        var existingItem = await _itemRepository.GetByIdAsync(request.Id);
        if (existingItem is null)
        {
            return Result<Item>.Failure(new Error("UpdateItemCostPrice.NotFound", "Item not found"));
        }

        existingItem.UpdateCostPrice(request.CostPrice);

        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(existingItem);
    }
}