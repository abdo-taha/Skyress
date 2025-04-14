using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(SkyressDbContext skyressDbContext) : base(skyressDbContext)
        {
        }
    }
}
