namespace Skyress.Application.Payments.Queries.GetAllPayments;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Common;

public record GetAllPaymentsQuery() : IQuery<List<Payment>>;

public class GetAllPaymentsQueryHandler : IQueryHandler<GetAllPaymentsQuery, List<Payment>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetAllPaymentsQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<List<Payment>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetAllAsync();
        return Result.Success(payments.ToList());
    }
}