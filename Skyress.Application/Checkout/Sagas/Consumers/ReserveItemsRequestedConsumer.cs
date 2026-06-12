using MassTransit;
using MediatR;
using Skyress.Application.Baskets.Commands.ReserveItems;
using Skyress.Application.Checkout.Events;
using Skyress.Domain.Common;

namespace Skyress.Application.Checkout.Sagas.Consumers;

public class ReserveItemsRequestedConsumer : IConsumer<ReserveItemsRequested>
{
    private readonly IMediator _mediator;

    public ReserveItemsRequestedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ReserveItemsRequested> context)
    {
        Result result = await _mediator.Send(new ReserveItemsCommand(context.Message.BasketId));
        if (result.IsFailure)
            return;

        await context.Publish(new ItemsReserved(context.Message.CorrelationId));
    }
}
