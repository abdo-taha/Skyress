using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Customers.Responses;
using Skyress.Domain.Common;

namespace Skyress.Application.Customers.Queries.GetCustomer;

public record GetCustomerQuery(long Id) : IQuery<CustomerResponse>;

public class GetCustomerQueryHandler : IQueryHandler<GetCustomerQuery, CustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomerQueryHandler> _logger;

    public GetCustomerQueryHandler(ICustomerRepository customerRepository, ILogger<GetCustomerQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<CustomerResponse>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetCustomerQuery));

        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
        {
            return Result<CustomerResponse>.Failure(new Error("GetCustomer.NotFound", "Customer not found"));
        }

        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(GetCustomerQuery), customer.Id);
        return Result.Success(CustomerResponse.FromDomain(customer));
    }
}
