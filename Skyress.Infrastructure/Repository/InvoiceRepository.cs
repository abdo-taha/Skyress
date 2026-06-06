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

        public async Task<Invoice?> GetByBasketIdAsync(long basketId, CancellationToken cancellationToken = default)
        {
            return await GetAsync(i => i.BasketId == basketId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Invoice?> GetByIdWithSoldItemsAsync(long invoiceId, CancellationToken cancellationToken = default)
        {
            return await SkyressDbContext.Invoices
                .Include(i => i.SoldItems)
                .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);
        }
    }
}
