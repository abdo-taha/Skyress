namespace Skyress.Application.Items.Commands.UpdateItemDescription;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

public record UpdateItemDescriptionCommand(
    long Id,
    string Description) : ICommand<Item>;

public class UpdateItemDescriptionCommandHandler(IItemRepository itemRepository) : ICommandHandler<UpdateItemDescriptionCommand, Item>
{
    public async Task<Result<Item>> Handle(UpdateItemDescriptionCommand request, CancellationToken cancellationToken)
    {
        var existingItem = await itemRepository.GetByIdAsync(request.Id);
        if (existingItem is null)
        {
            return Result<Item>.Failure(new Error("UpdateItemDescription.NotFound", "Item not found"));
        }

        existingItem.UpdateDescription(request.Description);

        await itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(existingItem);
    }
}