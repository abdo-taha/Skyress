using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Customers.Responses;
using Skyress.Domain.Common;

namespace Skyress.Application.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery() : IQuery<IReadOnlyList<CustomerResponse>>;

public class GetAllCustomersQueryHandler(ICustomerRepository customerRepository, ILogger<GetAllCustomersQueryHandler> logger)
    : IQueryHandler<GetAllCustomersQuery, IReadOnlyList<CustomerResponse>>
{
    private readonly ILogger<GetAllCustomersQueryHandler> _logger = logger;

    public async Task<Result<IReadOnlyList<CustomerResponse>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetAllCustomersQuery));

        var customers = await customerRepository.GetAllAsync(cancellationToken);
        var response = customers.Where(c => !c.IsDeleted).Select(CustomerResponse.FromDomain).ToList().AsReadOnly();
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetAllCustomersQuery), response.Count);
        return Result.Success<IReadOnlyList<CustomerResponse>>(response);
    }
}
