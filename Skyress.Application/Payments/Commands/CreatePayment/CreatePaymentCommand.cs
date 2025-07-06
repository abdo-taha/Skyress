namespace Skyress.Application.Payments.Commands.CreatePayment;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record CreatePaymentCommand(
    long InvoiceId,
    PaymentType PaymentType) : ICommand<Payment>;

public class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, Payment>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceRepository _invoiceRepository;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IInvoiceRepository invoiceRepository)
    {
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<Payment>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
        if (invoice is null)
        {
            return Result<Payment>.Failure(new Error("CreatePayment.InvoiceNotFound", "Invoice not found"));
        }

        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            TotalPaid = 0,
            TotalDue = invoice.TotalAmount,
            PaymentType = request.PaymentType,
            PaymentState = PaymentState.Initiated
        };

        var createdPayment = await _paymentRepository.CreateAsync(payment);
        await _paymentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success(createdPayment);
    }
}