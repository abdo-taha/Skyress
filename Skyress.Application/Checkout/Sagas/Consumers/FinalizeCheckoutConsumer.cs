using MassTransit;
using MediatR;
using Skyress.Application.Baskets.Commands.CompleteCheckout;
using Skyress.Application.Checkout.Events;
using Skyress.Application.Invoices.Commands.UpdateInvoiceState;
using Skyress.Application.Items.Commands.MarkItemsAsSold;
using Skyress.Domain.Enums;

namespace Skyress.Application.Checkout.Sagas.Consumers;

public class FinalizeCheckoutConsumer : IConsumer<FinalizeCheckoutRequested>
{
    private readonly IMediator _mediator;

    public FinalizeCheckoutConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<FinalizeCheckoutRequested> context)
    {
        await _mediator.Send(new UpdateInvoiceStateCommand(context.Message.InvoiceId, InvoiceState.Paid));
        await _mediator.Send(new CompleteCheckoutCommand(context.Message.BasketId));
        await _mediator.Send(new MarkItemsAsSoldCommand(context.Message.BasketId));
        await context.Publish(new FinalizedCheckout(context.Message.CorrelationId));
    }
}