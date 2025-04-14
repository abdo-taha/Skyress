using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Customer;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(SkyressDbContext shopDbContext) : base(shopDbContext)
        {
        }
    }
}
