using MassTransit;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Checkout.Events;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;
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
        {
            throw new Exception();
        }

        if (basket.InitiateCheckout().IsFailure)
        {
            throw new Exception();
        }

        Guid correlationId = this.UpdateCheckoutId(basket);
        
        await this._basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        await this._publisher.Publish(new CheckoutInitiated(correlationId, request.BasketId));
        
        return Result.Success();
    }

    private Guid UpdateCheckoutId(Basket basket)
    {
        if (Guid.TryParse(basket.CheckoutId, out Guid checkoutId) && checkoutId != Guid.Empty)
        {
            return checkoutId;
        }

        Guid newCheckoutId = Guid.NewGuid();
        basket.CheckoutId = newCheckoutId.ToString();
        return newCheckoutId;
    }
}