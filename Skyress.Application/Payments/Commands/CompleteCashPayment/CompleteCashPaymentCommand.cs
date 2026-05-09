using MassTransit;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<CompleteCashPaymentCommandHandler> _logger;

    public CompleteCashPaymentCommandHandler(IPaymentRepository paymentRepository, IPublishEndpoint publisher, ILogger<CompleteCashPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result> Handle(CompleteCashPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(CompleteCashPaymentCommand));

        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
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

        await _publisher.Publish(new PaymentCompletedEvent(request.PaymentId));
        _logger.LogInformation("{Command} completed", nameof(CompleteCashPaymentCommand));
        return Result.Success(payment);
    }
}
