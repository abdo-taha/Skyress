using System.Linq.Expressions;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

namespace Skyress.Application.Items.Queries.GetItemPricingHistory;

public record GetItemPricingHistoryQuery(long Id) : IQuery< List<PricingHistory>>;

public class GetItemPricingHistoryQueryHandler(IItemRepository itemRepository) : IQueryHandler<GetItemPricingHistoryQuery,  List<PricingHistory>>
{
    private readonly IItemRepository _itemRepository = itemRepository;

    public Task<Result< List<PricingHistory>>> Handle(GetItemPricingHistoryQuery request, CancellationToken cancellationToken)
    {
        Item? item = _itemRepository.GetAsync(predicate:(item) => item.Id == request.Id, includes: new List<Expression<Func<Item, object>>>()
        {
            item1 => item1.PricingHistory,
        }).FirstOrDefault();
        if (item is null)
        {
            return Task.FromResult(Result<List<PricingHistory>>.Failure(new Error("Not Found", "Not Found")));
        }
        return  Task.FromResult(Result.Success(item?.PricingHistory.ToList()));
    }
}