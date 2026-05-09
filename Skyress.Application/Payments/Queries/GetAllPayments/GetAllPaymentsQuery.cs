namespace Skyress.Application.Payments.Queries.GetAllPayments;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Payments.Responses;
using Skyress.Domain.Common;

public record GetAllPaymentsQuery() : IQuery<List<PaymentResponse>>;

public class GetAllPaymentsQueryHandler : IQueryHandler<GetAllPaymentsQuery, List<PaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<GetAllPaymentsQueryHandler> _logger;

    public GetAllPaymentsQueryHandler(IPaymentRepository paymentRepository, ILogger<GetAllPaymentsQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Result<List<PaymentResponse>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetAllPaymentsQuery));

        var payments = await _paymentRepository.GetAllAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetAllPaymentsQuery), payments.Count);
        return Result.Success(payments.Select(PaymentResponse.FromDomain).ToList());
    }
}
