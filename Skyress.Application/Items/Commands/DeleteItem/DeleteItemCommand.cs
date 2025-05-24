namespace Skyress.Application.Items.Commands.DeleteItem;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

public record DeleteItemCommand(long Id) : ICommand;

public class DeleteItemCommandHandler(IItemRepository itemRepository) : ICommandHandler<DeleteItemCommand>
{
    public async Task<Result> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(request.Id);
        if (item is null)
        {
            return Result.Failure(new Error("DeleteItem.NotFound", "Item not found"));
        }

        await itemRepository.DeleteByIdAsync(request.Id);
        await itemRepository.UnitOfWork.SaveChangesAsync();
        return Result.Success();
    }
} 