namespace Skyress.Application.Payments.Queries.GetPaymentById;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Payments.Responses;
using Skyress.Domain.Common;

public record GetPaymentByIdQuery(long Id) : IQuery<PaymentResponse>;

public class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, PaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<GetPaymentByIdQueryHandler> _logger;

    public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository, ILogger<GetPaymentByIdQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Result<PaymentResponse>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetPaymentByIdQuery));

        var payment = await _paymentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null)
        {
            return Result<PaymentResponse>.Failure(new Error("GetPaymentById.NotFound", "Payment not found"));
        }

        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(GetPaymentByIdQuery), payment.Id);
        return Result.Success(PaymentResponse.FromDomain(payment));
    }
}
