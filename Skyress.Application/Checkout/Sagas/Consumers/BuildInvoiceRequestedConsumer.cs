using MassTransit;
using MediatR;
using Skyress.Application.Checkout.Events;
using Skyress.Application.Invoices.Commands.BuildInvoiceFromBasketCommand;

namespace Skyress.Application.Checkout.Sagas.Consumers;

public class BuildInvoiceRequestedConsumer : IConsumer<BuildInvoiceRequested>
{
    private readonly IMediator _mediator;

    public BuildInvoiceRequestedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<BuildInvoiceRequested> context)
    {
        await _mediator.Send(new BuildInvoiceFromBasketCommand(context.Message.InvoiceId, context.Message.BasketId));
        await context.Publish(new InvoiceCreated(context.Message.CorrelationId));
    }
}