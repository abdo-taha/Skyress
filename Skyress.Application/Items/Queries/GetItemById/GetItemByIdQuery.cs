namespace Skyress.Application.Items.Queries.GetItemById;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Items.Responses;
using Skyress.Domain.Common;

public record GetItemByIdQuery(long Id) : IQuery<ItemResponse>;

public class GetItemByIdQueryHandler(IItemRepository itemRepository, ILogger<GetItemByIdQueryHandler> logger) : IQueryHandler<GetItemByIdQuery, ItemResponse>
{
    private readonly ILogger<GetItemByIdQueryHandler> _logger = logger;

    public async Task<Result<ItemResponse>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetItemByIdQuery));

        var item = await itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
        {
            return Result<ItemResponse>.Failure(new Error("GetItemById.NotFound", "Item not found"));
        }

        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(GetItemByIdQuery), item.Id);
        return Result.Success(ItemResponse.FromDomain(item));
    }
}
