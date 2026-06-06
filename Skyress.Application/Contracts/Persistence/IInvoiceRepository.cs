using Skyress.Domain.Aggregates.Invoice;

namespace Skyress.Application.Contracts.Persistence
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<Invoice?> GetByPaymentId(long paymentId);
        Task<Invoice?> GetByBasketIdAsync(long basketId, CancellationToken cancellationToken = default);
        Task<Invoice?> GetByIdWithSoldItemsAsync(long invoiceId, CancellationToken cancellationToken = default);
    }
}
