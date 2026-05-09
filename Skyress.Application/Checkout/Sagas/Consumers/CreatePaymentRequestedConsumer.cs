using MassTransit;
using MediatR;
using Skyress.Application.Checkout.Events;
using Skyress.Application.Payments.Commands.CreatePayment;
using Skyress.Application.Payments.Responses;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Checkout.Sagas.Consumers;

public class CreatePaymentRequestedConsumer : IConsumer<CreatePaymentRequested>
{
    private readonly IMediator _mediator;

    public CreatePaymentRequestedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CreatePaymentRequested> context)
    {
        Result<PaymentResponse> payment = await _mediator.Send(new CreatePaymentCommand(context.Message.InvoiceId, PaymentType.Cash));
        await context.Publish(new PaymentInitiated(context.Message.CorrelationId, payment.Value.Id));
    }
}