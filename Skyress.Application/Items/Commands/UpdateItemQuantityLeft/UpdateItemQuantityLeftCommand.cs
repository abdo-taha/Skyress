namespace Skyress.Application.Items.Commands.UpdateItemQuantityLeft;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Items.Responses;
using Skyress.Domain.Common;

public record UpdateItemQuantityLeftCommand(
    long Id,
    int QuantityLeft) : ICommand<ItemResponse>;

public class UpdateItemQuantityLeftCommandHandler : ICommandHandler<UpdateItemQuantityLeftCommand, ItemResponse>
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<UpdateItemQuantityLeftCommandHandler> _logger;

    public UpdateItemQuantityLeftCommandHandler(IItemRepository itemRepository, ILogger<UpdateItemQuantityLeftCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<Result<ItemResponse>> Handle(UpdateItemQuantityLeftCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateItemQuantityLeftCommand));

        var existingItem = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingItem is null)
        {
            return Result<ItemResponse>.Failure(new Error("UpdateItemQuantityLeft.NotFound", "Item not found"));
        }

        existingItem.UpdateQuantityLeft(request.QuantityLeft);

        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateItemQuantityLeftCommand));
        return Result.Success(ItemResponse.FromDomain(existingItem));
    }
}
