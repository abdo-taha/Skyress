namespace Skyress.Application.Payments.Queries.GetPaymentById;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Common;

public record GetPaymentByIdQuery(long Id) : IQuery<Payment>;

public class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, Payment>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<Payment>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.Id);
        if (payment is null)
        {
            return Result<Payment>.Failure(new Error("GetPaymentById.NotFound", "Payment not found"));
        }

        return Result.Success(payment);
    }
}