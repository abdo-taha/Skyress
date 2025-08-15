using MediatR;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Invoices.Commands.AddSoldItemToInvoice;
using Skyress.Application.Invoices.Commands.CreateInvoice;
using Skyress.Application.Payments.Commands.CreatePayment;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Baskets.Commands.InitiateCheckout;

public sealed class InitiateCheckoutCommandHandler : ICommandHandler<InitiateCheckoutCommand, long>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IMediator _mediator;

    public InitiateCheckoutCommandHandler(IBasketRepository basketRepository, IItemRepository itemRepository, IMediator mediator)
    {
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
        _mediator = mediator;
    }

    public async Task<Result<long>> Handle(InitiateCheckoutCommand request, CancellationToken cancellationToken)
    {

        Guid transactionId = Guid.NewGuid();
        using var transaction = await _basketRepository.UnitOfWork.BeginTransactionAsync(transactionId, cancellationToken);
        
        try
        {
            var basket = await ValidateBasketAsync(request.BasketId);

            await ReserveItemsAsync(basket);

            var invoice = await CreateInvoiceAsync(basket, cancellationToken);

            await AddSoldItemsToInvoiceAsync(basket, invoice, cancellationToken);

            Payment payment = await CreatePaymentAsync(invoice, cancellationToken);
            
            basket.AddPaymentId(payment.Id);
            
            await _basketRepository.UnitOfWork.CommitTransactionAsync(transactionId, cancellationToken);

            return Result.Success(payment.Id);
        }
        catch
        {
            await _basketRepository.UnitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<long>.Failure(Error.Dummy);
        }
    }

    private async Task<Basket> ValidateBasketAsync(long basketId)
    {
        var basket = await _basketRepository.GetBasketWithItemsAsync(basketId);
        if (basket is null)
        {
            throw new Exception();
        }

        if (basket.State != BasketState.Active && basket.State != BasketState.Cancelled)
        {
            throw new Exception();
        }

        if (!basket.BasketItems.Any())
        {
            throw new Exception();
        }

        return basket;
    }

    private async Task ReserveItemsAsync(Basket basket)
    {
        var itemIds = basket.BasketItems.Select(bi => bi.ItemId).ToList();
        var items = (await _itemRepository.GetByIdsAsync(itemIds)).ToDictionary(item => item.Id);
        
        foreach (var basketItem in basket.BasketItems)
        {
            if (!items.ContainsKey(basketItem.ItemId))
            {
                throw new Exception();
            }

            var item = items[basketItem.ItemId];
            var availableQuantity = item.QuantityLeft - item.QuantityReserved;
            
            if (availableQuantity < basketItem.Quantity)
            {
                throw new Exception();
            }
        }

        foreach (var basketItem in basket.BasketItems)
        {
            var item = items[basketItem.ItemId];
            var reserveResult = item.ReserveQuantity(basketItem.Quantity);
            if (reserveResult.IsFailure)
            {
                throw new InvalidOperationException(reserveResult.Error.Message);
            }
        }

        var basketInitiateCheckoutResult = basket.InitiateCheckout();
        if (basketInitiateCheckoutResult.IsFailure)
        {
            throw new InvalidOperationException(basketInitiateCheckoutResult.Error.Message);
        }
    }

    private async Task<Invoice> CreateInvoiceAsync(Basket basket, CancellationToken cancellationToken)
    {
        var createInvoiceCommand = new CreateInvoiceCommand(basket.UserId);
        var invoiceResult = await _mediator.Send(createInvoiceCommand, cancellationToken);

        if (invoiceResult.IsFailure)
        {
            throw new InvalidOperationException(invoiceResult.Error.Message);
        }

        return invoiceResult.Value;
    }

    private async Task AddSoldItemsToInvoiceAsync(Basket basket, Invoice invoice, CancellationToken cancellationToken)
    {
        foreach (var basketItem in basket.BasketItems)
        {
            var addSoldItemCommand = new AddSoldItemToInvoiceCommand(
                invoice.Id,
                basketItem.ItemId,
                basketItem.Quantity
            );

            var soldItemResult = await _mediator.Send(addSoldItemCommand, cancellationToken);
            if (soldItemResult.IsFailure)
            {
                throw new InvalidOperationException(soldItemResult.Error.Message);
            }
        }
    }

    private async Task<Payment> CreatePaymentAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        var createPaymentCommand = new CreatePaymentCommand(invoice.Id, PaymentType.Cash);
        var paymentResult = await _mediator.Send(createPaymentCommand, cancellationToken);

        if (paymentResult.IsFailure)
        {
            throw new InvalidOperationException(paymentResult.Error.Message);
        }
        return paymentResult.Value;
    }
}