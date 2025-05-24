namespace Skyress.Application.Items.Commands.CreateItem;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record CreateItemCommand(
    string Name,
    string Description,
    double Price,
    Unit Unit,
    double? CostPrice,
    int QuantityLeft,
    string? QrCode) : ICommand<Item>;

public class CreateItemCommandHandler(IItemRepository itemRepository) : ICommandHandler<CreateItemCommand, Item>
{
    public async Task<Result<Item>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {

        var item = Item.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Unit,
            request.QuantityLeft,
            request.CostPrice,
            request.QrCode
        );

        var createdItem = await itemRepository.CreateAsync(item);
        await itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(createdItem);
    }
}