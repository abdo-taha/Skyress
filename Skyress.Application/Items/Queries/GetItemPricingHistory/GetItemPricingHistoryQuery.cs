using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

namespace Skyress.Application.Items.Queries.GetItemPricingHistory;

public record GetItemPricingHistoryQuery(long Id) : IQuery<List<PricingHistory>>;

public class GetItemPricingHistoryQueryHandler(IItemRepository itemRepository, ILogger<GetItemPricingHistoryQueryHandler> logger) : IQueryHandler<GetItemPricingHistoryQuery, List<PricingHistory>>
{
    private readonly IItemRepository _itemRepository = itemRepository;
    private readonly ILogger<GetItemPricingHistoryQueryHandler> _logger = logger;

    public Task<Result<List<PricingHistory>>> Handle(GetItemPricingHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetItemPricingHistoryQuery));

        Item? item = _itemRepository.GetAsync(predicate: (item) => item.Id == request.Id, includes: new List<Expression<Func<Item, object>>>()
        {
            item1 => item1.PricingHistory,
        }).FirstOrDefault();
        if (item is null)
        {
            return Task.FromResult(Result<List<PricingHistory>>.Failure(new Error("Not Found", "Not Found")));
        }

        var result = item.PricingHistory.ToList();
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetItemPricingHistoryQuery), result.Count);
        return Task.FromResult(Result.Success(result));
    }
}
