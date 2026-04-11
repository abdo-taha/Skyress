using MassTransit;
using Skyress.Application.Checkout.Events;
using Microsoft.Extensions.Logging;

namespace Skyress.Application.Checkout.Sagas;

public class CheckoutStateMachine : MassTransitStateMachine<CheckoutSagaData>
{
    public State ReservingItems { get;  set; } = null!;
    
    public State InitiatingInvoice { get;  set; } = null!;
    
    public State BuildingInvoice { get;  set; } = null!;
    
    public State InitiatingPayment { get;  set; } = null!;
    
    public State PaymentPending { get;  set; } = null!;
    
    public State Finalizing { get;  set; } = null!;
    
    public State Completed { get;  set; } = null!;

    public Event<CheckoutInitiated> CheckoutInitiated { get;  set; } = null!;
    
    public Event<ItemsReserved> ItemsReservedEvent { get;  set; } = null!;
    
    public Event<InvoiceInitiated> InvoiceInitiatedEvent { get;  set; } = null!;
    
    public Event<InvoiceCreated> InvoiceCreatedEvent { get;  set; } = null!;
    
    public Event<PaymentInitiated> PaymentInitiatedEvent { get;  set; } = null!;
    
    public Event<CheckoutPaymentCompleted> PaymentCompletedEvent { get;  set; } = null!;
    
    public Event<FinalizedCheckout> FinalizedCheckoutEvent { get;  set; } = null!;
    

    private readonly ILogger<CheckoutStateMachine> _logger;

    public CheckoutStateMachine(ILogger<CheckoutStateMachine> logger)
    {
        _logger = logger;
        InstanceState(x => x.CurrentState);
        Event(() => CheckoutInitiated, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => ItemsReservedEvent, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => InvoiceInitiatedEvent, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => InvoiceCreatedEvent, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentCompletedEvent, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentInitiatedEvent, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => FinalizedCheckoutEvent, e => e.CorrelateById(m => m.Message.CorrelationId));
        
        // TODO reserving and selling items can be duplicate !! idemp..

        Initially(
            When(CheckoutInitiated)
                .Then(ctx =>
                {
                    _logger.LogInformation("Checkout Saga Started: {CorrelationId}, for Basket: {BasketId}", ctx.Saga.CorrelationId, ctx.Message.BasketId);
                    ctx.Saga.BasketId = ctx.Message.BasketId;
                })
                .Publish(ctx => new ReserveItemsRequested(ctx.Saga.CorrelationId, ctx.Saga.BasketId))
                .TransitionTo(ReservingItems)
        );

        During(ReservingItems,
            When(ItemsReservedEvent)
                .Then(ctx => _logger.LogInformation("Items Reserved for Saga: {CorrelationId}", ctx.Saga.CorrelationId))
                .Publish(ctx => new InitiateInvoiceRequested(ctx.Saga.CorrelationId, ctx.Saga.BasketId))
                .TransitionTo(InitiatingInvoice)
        );

        During(InitiatingInvoice,
            When(InvoiceInitiatedEvent)
                .Then(ctx =>
                {
                    _logger.LogInformation("Invoice Initiated for Saga: {CorrelationId}, InvoiceId: {InvoiceId}", ctx.Saga.CorrelationId, ctx.Message.InvoiceId);
                    ctx.Saga.InvoiceId = ctx.Message.InvoiceId;
                })
                .Publish(ctx => new BuildInvoiceRequested(ctx.Saga.CorrelationId, ctx.Saga.InvoiceId!.Value, ctx.Saga.BasketId))
                .TransitionTo(BuildingInvoice)
        );

        During(BuildingInvoice,
            When(InvoiceCreatedEvent)
                .Then(ctx => _logger.LogInformation("Invoice Created/Built for Saga: {CorrelationId}", ctx.Saga.CorrelationId))
                .Publish(ctx => new CreatePaymentRequested(ctx.Saga.CorrelationId, ctx.Saga.InvoiceId!.Value))
                .TransitionTo(InitiatingPayment)
        );

        During(InitiatingPayment,
            When(PaymentInitiatedEvent)
                .Then(ctx =>
                {
                    _logger.LogInformation("Payment Initiated for Saga: {CorrelationId}, PaymentId: {PaymentId}", ctx.Saga.CorrelationId, ctx.Message.PaymentId);
                    ctx.Saga.PaymentId = ctx.Message.PaymentId;
                })
                .TransitionTo(PaymentPending));
        
        During(PaymentPending,
            When(PaymentCompletedEvent)
                .Then(ctx => _logger.LogInformation("Payment Completed for Saga: {CorrelationId}", ctx.Saga.CorrelationId))
                .Publish(ctx => new FinalizeCheckoutRequested(ctx.Saga.CorrelationId, ctx.Saga.InvoiceId!.Value, ctx.Saga.BasketId))
                .TransitionTo(Finalizing)
        );
        
        During(Finalizing,
            When(FinalizedCheckoutEvent)
                .Then(ctx => _logger.LogInformation("Checkout Finalized and Saga Completed: {CorrelationId}", ctx.Saga.CorrelationId))
                .TransitionTo(Completed)
                .Finalize());

        SetCompletedWhenFinalized();
    }
}
