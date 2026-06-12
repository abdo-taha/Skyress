using MassTransit;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Checkout.Events;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;
using Skyress.Domain.Exceptions;
namespace Skyress.Application.Baskets.Commands.InitiateCheckout;

public sealed class InitiateCheckoutCommandHandler : ICommandHandler<InitiateCheckoutCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IPublishEndpoint _publisher;
    public InitiateCheckoutCommandHandler(IBasketRepository basketRepository, IPublishEndpoint publisher)
    {
        _basketRepository = basketRepository;
        _publisher = publisher;
    }

    public async Task<Result> Handle(InitiateCheckoutCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketWithItemsAsync(request.BasketId);
        if (basket is null)
            throw new Exception();

        // Already fully checked out — reject
        if (basket.State == BasketState.CheckedOut)
            return Result.Failure(new Error("Basket.AlreadyCheckedOut", "Basket has already been checked out."));

        // Already in progress — republish with existing correlation ID (idempotent re-entry)
        if (basket.State == BasketState.Reserved
            && !string.IsNullOrEmpty(basket.CheckoutId)
            && Guid.TryParse(basket.CheckoutId, out var existingId))
        {
            await _publisher.Publish(new CheckoutInitiated(existingId, request.BasketId));
            return Result.Success();
        }

        try
        {
            basket.InitiateCheckout();
        }
        catch (DomainException exception)
        {
            return DomainExceptionResultMapper.ToFailure(exception);
        }

        Guid correlationId = basket.EnsureCheckoutId();
        await this._basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await this._publisher.Publish(new CheckoutInitiated(correlationId, request.BasketId));
        return Result.Success();
    }
}
