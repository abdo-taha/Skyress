using MediatR;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Aggregates.Item.Events;

namespace Skyress.Application.Items.Events;

public class ItemPriceChangedDomainEventHandler(IItemRepository itemRepository)
    : INotificationHandler<ItemPriceChangedDomainEvent>
{
    public async Task Handle(ItemPriceChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        // this can be done during the update
        // it was a test for events flow
        var item = await itemRepository.GetByIdAsync(notification.ItemId);
        item?.AddPricingHistory(new PricingHistory(notification.ItemId, notification.OldPrice, notification.NewPrice, notification.OldCost, notification.NewCost, notification.PricingChangeType, notification.LastEditBy, notification.LastEditDate, notification.CreatedAt));
        await itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
} 