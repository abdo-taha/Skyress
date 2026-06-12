using MassTransit;
using Skyress.Application.Checkout.Events;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Payments.Events;
using Skyress.Domain.Aggregates.Basket;

namespace Skyress.Application.Checkout.Consumers;

public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
{
    private readonly IPublishEndpoint _publisher;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBasketRepository _basketRepository;
    
    public PaymentCompletedConsumer( IPublishEndpoint publisher, IInvoiceRepository repository, IBasketRepository basketRepository)
    {
        _publisher = publisher;
        _invoiceRepository = repository;
        _basketRepository = basketRepository;
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context) // TODO: what does this step do?
    {
        var invoice = await this._invoiceRepository.GetByPaymentId(context.Message.PaymentId);
        if (invoice == null)
        {
            return;
        }

        var basket = await this._basketRepository.GetByIdAsync(invoice.BasketId);
        if (basket is null || string.IsNullOrEmpty(basket.CheckoutId))
        {
            return;
        }

        await this._publisher.Publish(new CheckoutPaymentCompleted(Guid.Parse(basket.CheckoutId)));
    }
}
