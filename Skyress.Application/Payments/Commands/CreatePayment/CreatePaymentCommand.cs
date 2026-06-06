namespace Skyress.Application.Payments.Commands.CreatePayment;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Payments.Responses;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record CreatePaymentCommand(
    long InvoiceId,
    PaymentType PaymentType) : ICommand<PaymentResponse>;

public class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, PaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IInvoiceRepository invoiceRepository,
        ILogger<CreatePaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<Result<PaymentResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(CreatePaymentCommand));

        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result<PaymentResponse>.Failure(new Error("CreatePayment.InvoiceNotFound", "Invoice not found"));

        // Idempotency: return existing payment if one already exists for this invoice
        var existingPayment = await _paymentRepository.GetByInvoiceIdAsync(request.InvoiceId, cancellationToken);
        if (existingPayment is not null)
        {
            _logger.LogInformation("{Command} skipped — payment already exists. Id: {Id}",
                nameof(CreatePaymentCommand), existingPayment.Id);
            return Result.Success(PaymentResponse.FromDomain(existingPayment));
        }

        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            TotalPaid = 0,
            TotalDue = invoice.TotalAmount,
            PaymentType = request.PaymentType,
            PaymentState = PaymentState.Initiated
        };

        var createdPayment = await _paymentRepository.CreateAsync(payment, cancellationToken);
        await _paymentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        invoice.PaymentId = createdPayment.Id;
        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(CreatePaymentCommand), createdPayment.Id);
        return Result.Success(PaymentResponse.FromDomain(createdPayment));
    }
}
