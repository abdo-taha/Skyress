using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.DeleteBasketCommand;

public class DeleteBasketCommandHandler(IBasketRepository basketRepository, ILogger<DeleteBasketCommandHandler> logger) : ICommandHandler<DeleteBasketCommand>
{
    private readonly ILogger<DeleteBasketCommandHandler> _logger = logger;

    public async Task<Result> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command} for BasketId: {Id}", nameof(DeleteBasketCommand), request.BasketId);

        await basketRepository.DeleteByIdAsync(request.BasketId, cancellationToken);

        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. BasketId: {Id}", nameof(DeleteBasketCommand), request.BasketId);
        return Result.Success();
    }
}