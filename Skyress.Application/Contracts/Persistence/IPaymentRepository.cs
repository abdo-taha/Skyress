using Skyress.Domain.Aggregates.Payment;

namespace Skyress.Application.Contracts.Persistence;

public interface IPaymentRepository : IGenericRepository<Payment>
{
}
