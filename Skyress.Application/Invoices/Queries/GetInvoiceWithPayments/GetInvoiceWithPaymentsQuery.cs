using Skyress.Domain.Enums;

namespace Skyress.Application.Invoices.Queries.GetInvoiceWithPayments;

using Microsoft.EntityFrameworkCore;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Common;

public record GetInvoiceWithPaymentsQuery(long InvoiceId) : IQuery<InvoiceWithPaymentsDto>;

public class GetInvoiceWithPaymentsQueryHandler : IQueryHandler<GetInvoiceWithPaymentsQuery, InvoiceWithPaymentsDto>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;

    public GetInvoiceWithPaymentsQueryHandler(
        IInvoiceRepository invoiceRepository,
        IPaymentRepository paymentRepository)
    {
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<InvoiceWithPaymentsDto>> Handle(GetInvoiceWithPaymentsQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
        if (invoice is null)
        {
            return Result<InvoiceWithPaymentsDto>.Failure(new Error("GetInvoiceWithPayments.NotFound", "Invoice not found"));
        }

        var payments = _paymentRepository.GetAsync(
            predicate: p => p.InvoiceId == request.InvoiceId,
            disableTracking: true);

        var paymentsList = await payments.ToListAsync(cancellationToken);

        var invoiceWithPayments = new InvoiceWithPaymentsDto
        {
            Id = invoice.Id,
            TotalAmount = invoice.TotalAmount,
            CustomerId = invoice.CustomerId,
            State = invoice.State,
            CreatedAt = invoice.CreatedAt,
            LastEditDate = invoice.LastEditDate,
            Payments = paymentsList.Select(Convert).ToList()
        };

        return Result.Success(invoiceWithPayments);
    }
    
    private PaymentDto Convert(Payment payment)
    {
        return new PaymentDto()
        {
            Id = payment.Id,
            PaymentType = payment.PaymentType,
            PaymentState = payment.PaymentState,
            TotalPaid = payment.TotalPaid,
            InvoiceId = payment.InvoiceId,
            TotalDue = payment.TotalDue,
        };
    }
}

public class InvoiceWithPaymentsDto
{
    public long Id { get; set; }
    public decimal TotalAmount { get; set; }
    public long? CustomerId { get; set; }
    public InvoiceState State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastEditDate { get; set; }
    public List<PaymentDto> Payments { get; set; } = new();
}

public class PaymentDto
{
    public long Id { get; set; }
    
    public PaymentType PaymentType { get; set; }

    public PaymentState PaymentState { get; set; }

    public decimal TotalPaid { get; set; }

    public long InvoiceId { get; set; }

    public decimal TotalDue { get; set; }
}