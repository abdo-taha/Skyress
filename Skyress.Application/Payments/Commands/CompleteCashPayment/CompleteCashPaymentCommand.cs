using MassTransit;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Payments.Events;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Payments.Commands.CompleteCashPayment;


public record CompleteCashPaymentCommand(long PaymentId, decimal TotalPaid) : ICommand;

public class CompleteCashPaymentCommandHandler : ICommandHandler<CompleteCashPaymentCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPublishEndpoint _publisher;
    public CompleteCashPaymentCommandHandler(IPaymentRepository paymentRepository, IPublishEndpoint publisher)
    {
        _paymentRepository = paymentRepository;
        _publisher = publisher;
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

        payment.PaymentState = PaymentState.Paid;
        payment.TotalPaid = payment.TotalDue;

        await _paymentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        await this._publisher.Publish(new PaymentCompletedEvent(request.PaymentId));
        return Result.Success(payment);
    }

}