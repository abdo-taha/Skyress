using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Common;

namespace Skyress.Application.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery() : IQuery<List<Customer>>;

public class GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
    : IQueryHandler<GetAllCustomersQuery, List<Customer>>
{
    public async Task<Result<List<Customer>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await customerRepository.GetAllAsync();
        return Result.Success(customers.Where(c => !c.IsDeleted).ToList());
    }
}