namespace Skyress.Application.Items.Commands.UpdateItemQrCode;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Items.Responses;
using Skyress.Domain.Common;

public record UpdateItemQrCodeCommand(
    long Id,
    string? QrCode) : ICommand<ItemResponse>;

public class UpdateItemQrCodeCommandHandler : ICommandHandler<UpdateItemQrCodeCommand, ItemResponse>
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<UpdateItemQrCodeCommandHandler> _logger;

    public UpdateItemQrCodeCommandHandler(IItemRepository itemRepository, ILogger<UpdateItemQrCodeCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<Result<ItemResponse>> Handle(UpdateItemQrCodeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateItemQrCodeCommand));

        var existingItem = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingItem is null)
        {
            return Result<ItemResponse>.Failure(new Error("UpdateItemQrCode.NotFound", "Item not found"));
        }

        existingItem.UpdateQrCode(request.QrCode);

        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateItemQrCodeCommand));
        return Result.Success(ItemResponse.FromDomain(existingItem));
    }
}
