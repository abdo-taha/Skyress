using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.ClearBasket;

public class ClearBasketCommandHandler(IBasketRepository basketRepository, ILogger<ClearBasketCommandHandler> logger) : ICommandHandler<ClearBasketCommand>
{
    private readonly ILogger<ClearBasketCommandHandler> _logger = logger;

    public async Task<Result> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(ClearBasketCommand));

        var basket = await basketRepository.GetBasketWithItemsAsync(request.BasketId);

        if (basket is null)
        {
            return Result.Failure(new Error("Basket.NotFound", "The basket was not found."));
        }

        basket.Clear();

        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(ClearBasketCommand));
        return Result.Success();
    }
}