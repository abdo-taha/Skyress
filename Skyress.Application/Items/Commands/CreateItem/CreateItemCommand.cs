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
    double? CostPrice,
    int QuantityLeft,
    string? QrCode,
    Unit Unit) : ICommand<Item>;

// todo move to file
public class CreateItemCommandHandler(IItemRepository itemRepository) : ICommandHandler<CreateItemCommand, Item>
{
    public async Task<Result<Item>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = new Item
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CostPrice = request.CostPrice,
                QuantityLeft = request.QuantityLeft,
                QuantitySold = 0,
                QrCode = request.QrCode,
                Unit = request.Unit,
                IsDeleted = false,
                LastEditDate = DateTime.UtcNow,
                CreaedAt = DateTime.UtcNow
            };

            var createdItem = await itemRepository.CreateAsync(item);
            await itemRepository.UnitOfWork.SaveChangesAsync();
            return Result.Success(createdItem);
        }
        catch (Exception ex)
        {
            // todo add errors
            return Result<Item>.Failure(new Error("CreateItem.Error", ex.Message));
        }
    }
} 