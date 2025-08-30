using MediatR;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Items.Commands.MarkItemsAsSold;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Payments.Commands.CompleteCashPayment;

public record CompleteCashPaymentCommand(long PaymentId, decimal TotalPaid) : ICommand;

public class CompleteCashPaymentCommandHandler : ICommandHandler<CompleteCashPaymentCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBasketRepository _basketRepository;
    private readonly ISender _sender;

    public CompleteCashPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IInvoiceRepository invoiceRepository,
        IBasketRepository basketRepository, ISender sender)
    {
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
        _basketRepository = basketRepository;
        _sender = sender;
    }

    public async Task<Result> Handle(CompleteCashPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId);
        if (payment is null)
        {
            return Result.Failure(Error.Dummy);
        }

        if (payment.PaymentType != PaymentType.Cash)
        {
            return Result.Failure(Error.Dummy);
        }

        if (payment.PaymentState != PaymentState.Initiated)
        {
            return Result.Failure(Error.Dummy);
        }

        if (payment.TotalDue != request.TotalPaid)
        {
            return Result.Failure(Error.Dummy);
        }

        var invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceId);
        if (invoice is null)
        {
            return Result.Failure(Error.Dummy);
        }
        
        Guid transactionId = Guid.NewGuid();
        using var transaction = await _paymentRepository.UnitOfWork.BeginTransactionAsync(transactionId, cancellationToken);

        try
        {
            payment.PaymentState = PaymentState.Paid;
            payment.TotalPaid = payment.TotalDue;
            invoice.State = InvoiceState.Paid;
            
            var basket = await _basketRepository.GetBasketByPaymentIdAsync(request.PaymentId);
            if (basket is null)
            {
                return Result.Failure(Error.Dummy);
            }

            basket.CompleteCheckout();
            await SellItemsAsync(basket);

            await _paymentRepository.UnitOfWork.CommitTransactionAsync(transactionId, cancellationToken);

            return Result.Success(payment);
        }
        catch (Exception)
        {
            await _paymentRepository.UnitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(Error.Dummy);
        }
    }
    
    private async Task SellItemsAsync(Basket basket)
    {
        Dictionary<long, int> itemQuantities = new Dictionary<long, int>();
        foreach (var item in basket.BasketItems)
        {
            itemQuantities[item.ItemId] = item.Quantity;
        }
        MarkItemsAsSoldCommand command = new MarkItemsAsSoldCommand(itemQuantities);
        await this._sender.Send(command);
    }
}