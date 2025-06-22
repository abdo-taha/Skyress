using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Common;

namespace Skyress.Application.Customers.Queries.GetCustomer;

public record GetCustomerQuery(long Id) : IQuery<Customer>;

public class GetCustomerQueryHandler : IQueryHandler<GetCustomerQuery, Customer>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<Customer>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        if (customer is null)
        {
            return Result<Customer>.Failure(new Error("GetCustomer.NotFound", "Customer not found"));
        }

        return Result.Success(customer);
    }
}