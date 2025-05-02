namespace Skyress.Application.Items.Queries.GetItemById;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;

public record GetItemByIdQuery(long Id) : IQuery<Item>;

// todo move to file
// todo add errors
public class GetItemByIdQueryHandler(IItemRepository itemRepository) : IQueryHandler<GetItemByIdQuery, Item>
{
    public async Task<Result<Item>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await itemRepository.GetByIdAsync(request.Id);
            if (item is null)
            {
                return Result<Item>.Failure(new Error("GetItemById.NotFound", "Item not found"));
            }

            return Result.Success(item);
        }
        catch (Exception ex)
        {
            return Result<Item>.Failure(new Error("GetItemById.Error", ex.Message));
        }
    }
} 