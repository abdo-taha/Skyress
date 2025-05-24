namespace Skyress.Application.Items.Commands.UpdateItemUnit;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record UpdateItemUnitCommand(
    long Id,
    Unit Unit) : ICommand<Item>;

public class UpdateItemUnitCommandHandler : ICommandHandler<UpdateItemUnitCommand, Item>
{
    private readonly IItemRepository _itemRepository;

    public UpdateItemUnitCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Result<Item>> Handle(UpdateItemUnitCommand request, CancellationToken cancellationToken)
    {
        var existingItem = await _itemRepository.GetByIdAsync(request.Id);
        if (existingItem is null)
        {
            return Result<Item>.Failure(new Error("UpdateItemUnit.NotFound", "Item not found"));
        }

        existingItem.UpdateUnit(request.Unit);
        
        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(existingItem);
    }
}