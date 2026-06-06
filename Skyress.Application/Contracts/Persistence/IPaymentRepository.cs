using Skyress.Domain.Aggregates.Payment;

namespace Skyress.Application.Contracts.Persistence;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> GetByInvoiceIdAsync(long invoiceId, CancellationToken cancellationToken = default);
}
