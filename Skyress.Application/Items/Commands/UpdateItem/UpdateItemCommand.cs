namespace Skyress.Application.Items.Commands.UpdateItem;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record UpdateItemCommand(
    long Id,
    string? Name,
    string? Description,
    double? Price,
    double? CostPrice,
    int? QuantityLeft,
    string? QrCode,
    Unit? Unit) : ICommand<Item>;

// todo move to file
// todo add errors
public class UpdateItemCommandHandler(IItemRepository itemRepository) : ICommandHandler<UpdateItemCommand, Item>
{
    public async Task<Result<Item>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingItem = await itemRepository.GetByIdAsync(request.Id);
            if (existingItem is null)
            {
                return Result<Item>.Failure(new Error("UpdateItem.NotFound", "Item not found"));
            }

            var item = new Item
            {
                Id = request.Id,
                Name = request.Name ?? existingItem.Name,
                Description = request.Description ?? existingItem.Description,
                Price = request.Price ?? existingItem.Price,
                CostPrice = request.CostPrice ?? existingItem.CostPrice,
                QuantityLeft = request.QuantityLeft ?? existingItem.QuantityLeft,
                QuantitySold = existingItem.QuantitySold,
                QrCode = request.QrCode ?? existingItem.QrCode,
                Unit = request.Unit ?? existingItem.Unit,
                IsDeleted = existingItem.IsDeleted,
                LastEditDate = DateTime.UtcNow,
                CreaedAt = existingItem.CreaedAt
            };

            var updatedItem = await itemRepository.UpdateAsync(item);
            return Result.Success(updatedItem);
        }
        catch (Exception ex)
        {
            return Result<Item>.Failure(new Error("UpdateItem.Error", ex.Message));
        }
    }
} 