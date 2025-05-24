namespace Skyress.Application.Items.Commands.UpdateItemName;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

public record UpdateItemNameCommand(
    long Id,
    string Name) : ICommand<Item>;

public class UpdateItemNameCommandHandler : ICommandHandler<UpdateItemNameCommand, Item>
{
    private readonly IItemRepository _itemRepository;

    public UpdateItemNameCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Result<Item>> Handle(UpdateItemNameCommand request, CancellationToken cancellationToken)
    {
        var existingItem = await _itemRepository.GetByIdAsync(request.Id);
        if (existingItem is null)
        {
            return Result<Item>.Failure(new Error("UpdateItemName.NotFound", "Item not found"));
        }

        existingItem.UpdateName(request.Name);


        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(existingItem);
    }
}