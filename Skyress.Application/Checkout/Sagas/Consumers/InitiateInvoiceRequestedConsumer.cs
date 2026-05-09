using MassTransit;
using MediatR;
using Skyress.Application.Checkout.Events;
using Skyress.Application.Invoices.Commands.CreateInvoice;
using Skyress.Application.Invoices.Responses;
using Skyress.Domain.Common;

namespace Skyress.Application.Checkout.Sagas.Consumers;

public class InitiateInvoiceRequestedConsumer : IConsumer<InitiateInvoiceRequested>
{
    private readonly IMediator _mediator;

    public InitiateInvoiceRequestedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<InitiateInvoiceRequested> context)
    {
        Result<InvoiceResponse> result = await _mediator.Send(new CreateInvoiceCommand(context.Message.BasketId));
        if (result.IsFailure)
            throw new InvalidOperationException(result.Error.Message);
        await context.Publish(new InvoiceInitiated(context.Message.CorrelationId, result.Value.Id));
    }
}