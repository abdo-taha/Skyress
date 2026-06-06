using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Baskets.Commands.CompleteCheckout;

public sealed record CompleteCheckoutCommand(long BasketId) : ICommand;

public sealed class CompleteCheckoutCommandHandler : ICommandHandler<CompleteCheckoutCommand>
{
    private readonly IBasketRepository _basketRepository;

    public CompleteCheckoutCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result> Handle(CompleteCheckoutCommand request, CancellationToken cancellationToken)
    {
        Result<Basket> result = await this._basketRepository.GetBasketWithItemsAsync(request.BasketId);

        // Idempotency: no-op if already checked out
        if (result.Value.State == BasketState.CheckedOut)
            return Result.Success();

        result.Value.CompleteCheckout();
        await this._basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}