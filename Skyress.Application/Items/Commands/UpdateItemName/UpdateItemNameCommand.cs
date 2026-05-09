namespace Skyress.Application.Items.Commands.UpdateItemName;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Items.Responses;
using Skyress.Domain.Common;

public record UpdateItemNameCommand(
    long Id,
    string Name) : ICommand<ItemResponse>;

public class UpdateItemNameCommandHandler : ICommandHandler<UpdateItemNameCommand, ItemResponse>
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<UpdateItemNameCommandHandler> _logger;

    public UpdateItemNameCommandHandler(IItemRepository itemRepository, ILogger<UpdateItemNameCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<Result<ItemResponse>> Handle(UpdateItemNameCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateItemNameCommand));

        var existingItem = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingItem is null)
        {
            return Result<ItemResponse>.Failure(new Error("UpdateItemName.NotFound", "Item not found"));
        }

        existingItem.UpdateName(request.Name);

        await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateItemNameCommand));
        return Result.Success(ItemResponse.FromDomain(existingItem));
    }
}
