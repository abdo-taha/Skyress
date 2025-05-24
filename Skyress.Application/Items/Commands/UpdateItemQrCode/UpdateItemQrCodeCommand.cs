namespace Skyress.Application.Items.Commands.UpdateItemQrCode;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

public record UpdateItemQrCodeCommand(
    long Id,
    string? QrCode) : ICommand<Item>;

public class UpdateItemQrCodeCommandHandler : ICommandHandler<UpdateItemQrCodeCommand, Item>
{
    private readonly IItemRepository _itemRepository;

    public UpdateItemQrCodeCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Result<Item>> Handle(UpdateItemQrCodeCommand request, CancellationToken cancellationToken)
    {
        var existingItem = await _itemRepository.GetByIdAsync(request.Id);
        if (existingItem is null)
        {
            return Result<Item>.Failure(new Error("UpdateItemQrCode.NotFound", "Item not found"));
        }
        
        existingItem.UpdateQrCode(request.QrCode);

        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(existingItem);
    }
}