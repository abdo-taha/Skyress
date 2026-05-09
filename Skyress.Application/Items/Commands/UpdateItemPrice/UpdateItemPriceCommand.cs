namespace Skyress.Application.Items.Commands.UpdateItemPrice;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Items.Responses;
using Skyress.Domain.Common;

public record UpdateItemPriceCommand(
    long Id,
    decimal? Price,
    decimal? CostPrice) : ICommand<ItemResponse>;

public class UpdateItemPriceCommandHandler : ICommandHandler<UpdateItemPriceCommand, ItemResponse>
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<UpdateItemPriceCommandHandler> _logger;

    public UpdateItemPriceCommandHandler(IItemRepository itemRepository, ILogger<UpdateItemPriceCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<Result<ItemResponse>> Handle(UpdateItemPriceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateItemPriceCommand));

        var existingItem = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingItem is null)
        {
            return Result<ItemResponse>.Failure(new Error("UpdateItemPrice.NotFound", "Item not found"));
        }

        existingItem.UpdatePrice(request.Price, request.CostPrice);

        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateItemPriceCommand));
        return Result.Success(ItemResponse.FromDomain(existingItem));
    }
}
