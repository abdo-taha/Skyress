namespace Skyress.Application.Items.Commands.CreateItem;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Items.Responses;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record CreateItemCommand(
    string Name,
    string Description,
    decimal Price,
    Unit Unit,
    decimal? CostPrice,
    int QuantityLeft,
    string? QrCode) : ICommand<ItemResponse>;

public class CreateItemCommandHandler(IItemRepository itemRepository, ILogger<CreateItemCommandHandler> logger) : ICommandHandler<CreateItemCommand, ItemResponse>
{
    private readonly ILogger<CreateItemCommandHandler> _logger = logger;

    public async Task<Result<ItemResponse>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(CreateItemCommand));

        var item = Item.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Unit,
            request.QuantityLeft,
            request.CostPrice,
            request.QrCode
        );

        var createdItem = await itemRepository.CreateAsync(item, cancellationToken);
        await itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(CreateItemCommand), createdItem.Id);
        return Result.Success(ItemResponse.FromDomain(createdItem));
    }
}
