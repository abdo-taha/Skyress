namespace Skyress.Application.Items.Commands.DeleteItem;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

public record DeleteItemCommand(long Id) : ICommand;

public class DeleteItemCommandHandler(IItemRepository itemRepository, ILogger<DeleteItemCommandHandler> logger) : ICommandHandler<DeleteItemCommand>
{
    private readonly ILogger<DeleteItemCommandHandler> _logger = logger;

    public async Task<Result> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command} for ItemId: {Id}", nameof(DeleteItemCommand), request.Id);

        var item = await itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
        {
            return Result.Failure(new Error("DeleteItem.NotFound", "Item not found"));
        }

        await itemRepository.DeleteByIdAsync(request.Id, cancellationToken);
        await itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. ItemId: {Id}", nameof(DeleteItemCommand), request.Id);
        return Result.Success();
    }
}
