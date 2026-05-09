namespace Skyress.Application.Items.Commands.UpdateItemUnit;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Items.Responses;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record UpdateItemUnitCommand(
    long Id,
    Unit Unit) : ICommand<ItemResponse>;

public class UpdateItemUnitCommandHandler : ICommandHandler<UpdateItemUnitCommand, ItemResponse>
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<UpdateItemUnitCommandHandler> _logger;

    public UpdateItemUnitCommandHandler(IItemRepository itemRepository, ILogger<UpdateItemUnitCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<Result<ItemResponse>> Handle(UpdateItemUnitCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateItemUnitCommand));

        var existingItem = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingItem is null)
        {
            return Result<ItemResponse>.Failure(new Error("UpdateItemUnit.NotFound", "Item not found"));
        }

        existingItem.UpdateUnit(request.Unit);

        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateItemUnitCommand));
        return Result.Success(ItemResponse.FromDomain(existingItem));
    }
}
