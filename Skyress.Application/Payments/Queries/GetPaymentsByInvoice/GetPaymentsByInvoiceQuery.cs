namespace Skyress.Application.Payments.Queries.GetPaymentsByInvoice;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Payments.Responses;
using Skyress.Domain.Common;

public record GetPaymentsByInvoiceQuery(long InvoiceId) : IQuery<List<PaymentResponse>>;

public class GetPaymentsByInvoiceQueryHandler : IQueryHandler<GetPaymentsByInvoiceQuery, List<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<GetPaymentsByInvoiceQueryHandler> _logger;

    public GetPaymentsByInvoiceQueryHandler(IPaymentRepository paymentRepository, ILogger<GetPaymentsByInvoiceQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Result<List<PaymentResponse>>> Handle(GetPaymentsByInvoiceQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetPaymentsByInvoiceQuery));

        var payments = _paymentRepository.GetAsync(
            predicate: p => p.InvoiceId == request.InvoiceId,
            disableTracking: true);

        var paymentsList = await payments.ToListAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetPaymentsByInvoiceQuery), paymentsList.Count);
        return Result.Success(paymentsList.Select(PaymentResponse.FromDomain).ToList());
    }
}
