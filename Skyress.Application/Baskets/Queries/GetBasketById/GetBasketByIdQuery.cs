using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;
using Skyress.Application.Baskets.DTOs;

namespace Skyress.Application.Baskets.Queries.GetBasketById;

public record GetBasketByIdQuery(long Id) : IQuery<BasketDto>;

public class GetBasketByIdQueryHandler(IBasketRepository basketRepository, IItemRepository itemRepository) : IQueryHandler<GetBasketByIdQuery, BasketDto>
{
    public async Task<Result<BasketDto>> Handle(GetBasketByIdQuery request, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetBasketWithItemsAsync(request.Id);

        if (basket is null)
        {
            return Result<BasketDto>.Failure(Error.Dummy);
        }

        var itemIds = basket.BasketItems.Select(bi => bi.ItemId).ToList();
        var items = (await itemRepository.GetByIdsAsync(itemIds)).ToDictionary(item => item.Id);

        var basketDto = new BasketDto
        {
            Id = basket.Id,
            UserId = basket.UserId,
            State = basket.State,
            Items = basket.BasketItems.Select(bi => new BasketItemDto
            {
                Id = bi.Id,
                ItemId = bi.ItemId,
                Quantity = bi.Quantity,
                Price = items.TryGetValue(bi.ItemId, out var item) ? item.Price : 0,
                Name = items.TryGetValue(bi.ItemId, out item) ? item.Name : null,
                Unit = items.TryGetValue(bi.ItemId, out item) ? item.Unit : null,
            }).ToList()
        };

        return Result.Success(basketDto);
    }
}