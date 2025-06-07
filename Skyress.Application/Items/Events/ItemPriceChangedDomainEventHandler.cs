using MediatR;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Aggregates.Item.Events;

namespace Skyress.Application.Items.Events;

public class ItemPriceChangedDomainEventHandler : INotificationHandler<ItemPriceChangedDomainEvent>
{
    private IItemRepository _itemRepository;

    public ItemPriceChangedDomainEventHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task Handle(ItemPriceChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var item = await this._itemRepository.GetByIdAsync(notification.ItemId);
        item?.AddPricingHistory(new PricingHistory(notification.ItemId, notification.OldPrice, notification.NewPrice, notification.OldCost, notification.NewCost, notification.PricingChangeType, notification.LastEditBy, notification.LastEditDate, notification.CreatedAt));
    }
} 