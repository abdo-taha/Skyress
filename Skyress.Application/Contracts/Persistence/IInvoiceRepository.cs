using Skyress.Domain.Aggregates.Invoice;

namespace Skyress.Application.Contracts.Persistence
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
    }
}
