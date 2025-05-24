namespace Skyress.Application.Items.Queries.GetAllItems;

using Contracts.Persistence;
using Domain.Aggregates.Item;
using Domain.Common;
using Abstractions.Messaging;

public record GetAllItemsQuery : IQuery<IReadOnlyList<Item>>;

public class GetAllItemsQueryHandler(IItemRepository itemRepository)
    : IQueryHandler<GetAllItemsQuery, IReadOnlyList<Item>>
{
    public async Task<Result<IReadOnlyList<Item>>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await itemRepository.GetAllAsync();
        return Result.Success(items);
    }
}