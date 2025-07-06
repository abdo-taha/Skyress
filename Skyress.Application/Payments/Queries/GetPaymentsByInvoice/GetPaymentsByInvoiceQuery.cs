namespace Skyress.Application.Payments.Queries.GetPaymentsByInvoice;

using Microsoft.EntityFrameworkCore;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Common;

public record GetPaymentsByInvoiceQuery(long InvoiceId) : IQuery<List<Payment>>;

public class GetPaymentsByInvoiceQueryHandler : IQueryHandler<GetPaymentsByInvoiceQuery, List<Payment>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentsByInvoiceQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<List<Payment>>> Handle(GetPaymentsByInvoiceQuery request, CancellationToken cancellationToken)
    {
        var payments = _paymentRepository.GetAsync(
            predicate: p => p.InvoiceId == request.InvoiceId,
            disableTracking: true);

        var paymentsList = await payments.ToListAsync(cancellationToken);
        return Result.Success(paymentsList);
    }
}