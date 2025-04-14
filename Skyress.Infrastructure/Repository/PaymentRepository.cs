using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(SkyressDbContext skyressDbContext) : base(skyressDbContext)
    {
    }
}
