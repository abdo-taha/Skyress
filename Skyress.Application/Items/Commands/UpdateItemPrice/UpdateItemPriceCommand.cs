namespace Skyress.Application.Items.Commands.UpdateItemPrice;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

public record UpdateItemPriceCommand(
    long Id,
    decimal Price) : ICommand<Item>;

public class UpdateItemPriceCommandHandler : ICommandHandler<UpdateItemPriceCommand, Item>
{
    private readonly IItemRepository _itemRepository;

    public UpdateItemPriceCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Result<Item>> Handle(UpdateItemPriceCommand request, CancellationToken cancellationToken)
    {
        var existingItem = await _itemRepository.GetByIdAsync(request.Id);
        if (existingItem is null)
        {
            return Result<Item>.Failure(new Error("UpdateItemPrice.NotFound", "Item not found"));
        }

        existingItem.UpdatePrice(request.Price);
        
        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(existingItem);
    }
}