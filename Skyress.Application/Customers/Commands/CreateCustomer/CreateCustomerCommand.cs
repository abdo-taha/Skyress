namespace Skyress.Application.Customers.Commands.CreateCustomer;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Customers.Responses;
using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record CreateCustomerCommand(
    string Name,
    string Notes,
    CustomerState State) : ICommand<CustomerResponse>;

public class CreateCustomerCommandHandler(ICustomerRepository customerRepository, ILogger<CreateCustomerCommandHandler> logger)
    : ICommandHandler<CreateCustomerCommand, CustomerResponse>
{
    private readonly ILogger<CreateCustomerCommandHandler> _logger = logger;

    public async Task<Result<CustomerResponse>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(CreateCustomerCommand));

        var customer = Customer.Create(request.Name, request.Notes, request.State);

        var createdCustomer = await customerRepository.CreateAsync(customer, cancellationToken);
        await customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(CreateCustomerCommand), createdCustomer.Id);
        return Result.Success(CustomerResponse.FromDomain(createdCustomer));
    }
}
