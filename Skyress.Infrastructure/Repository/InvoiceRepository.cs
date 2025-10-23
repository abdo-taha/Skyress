using Microsoft.EntityFrameworkCore;
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

        public async Task<Invoice?> GetByPaymentId(long paymentId)
        {
            return await GetAsync(i => i.PaymentId == paymentId).FirstOrDefaultAsync();
        }
    }
}
