namespace Skyress.Application.Items.Queries.GetAllItems;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Items.Responses;
using Skyress.Domain.Common;

public record GetAllItemsQuery : IQuery<IReadOnlyList<ItemResponse>>;

public class GetAllItemsQueryHandler(IItemRepository itemRepository, ILogger<GetAllItemsQueryHandler> logger)
    : IQueryHandler<GetAllItemsQuery, IReadOnlyList<ItemResponse>>
{
    private readonly ILogger<GetAllItemsQueryHandler> _logger = logger;

    public async Task<Result<IReadOnlyList<ItemResponse>>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetAllItemsQuery));

        var items = await itemRepository.GetAllAsync(cancellationToken);
        var response = items.Select(ItemResponse.FromDomain).ToList().AsReadOnly();
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetAllItemsQuery), response.Count);
        return Result.Success<IReadOnlyList<ItemResponse>>(response);
    }
}
