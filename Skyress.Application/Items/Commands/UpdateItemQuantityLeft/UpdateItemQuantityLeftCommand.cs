namespace Skyress.Application.Items.Commands.UpdateItemQuantityLeft;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

public record UpdateItemQuantityLeftCommand(
    long Id,
    int QuantityLeft) : ICommand<Item>;

public class UpdateItemQuantityLeftCommandHandler : ICommandHandler<UpdateItemQuantityLeftCommand, Item>
{
    private readonly IItemRepository _itemRepository;

    public UpdateItemQuantityLeftCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Result<Item>> Handle(UpdateItemQuantityLeftCommand request, CancellationToken cancellationToken)
    {
        var existingItem = await _itemRepository.GetByIdAsync(request.Id);
        if (existingItem is null)
        {
            return Result<Item>.Failure(new Error("UpdateItemQuantityLeft.NotFound", "Item not found"));
        }


        existingItem.UpdateQuantityLeft(request.QuantityLeft);

        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(existingItem);
    }
}