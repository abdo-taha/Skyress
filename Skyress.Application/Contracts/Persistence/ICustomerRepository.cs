using Skyress.Domain.Aggregates.Customer;

namespace Skyress.Application.Contracts.Persistence
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
    }
}
