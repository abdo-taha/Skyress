namespace Skyress.Application.Items.Commands.UpdateItemDescription;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Items.Responses;
using Skyress.Domain.Common;

public record UpdateItemDescriptionCommand(
    long Id,
    string Description) : ICommand<ItemResponse>;

public class UpdateItemDescriptionCommandHandler(IItemRepository itemRepository, ILogger<UpdateItemDescriptionCommandHandler> logger) : ICommandHandler<UpdateItemDescriptionCommand, ItemResponse>
{
    private readonly ILogger<UpdateItemDescriptionCommandHandler> _logger = logger;

    public async Task<Result<ItemResponse>> Handle(UpdateItemDescriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateItemDescriptionCommand));

        var existingItem = await itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingItem is null)
        {
            return Result<ItemResponse>.Failure(new Error("UpdateItemDescription.NotFound", "Item not found"));
        }

        existingItem.UpdateDescription(request.Description);

        await itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateItemDescriptionCommand));
        return Result.Success(ItemResponse.FromDomain(existingItem));
    }
}
