namespace Skyress.Application.Customers.Commands.UpdateCustomerState;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Customers.Responses;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record UpdateCustomerStateCommand(
    long Id,
    CustomerState State) : ICommand<CustomerResponse>;

public class UpdateCustomerStateCommandHandler(ICustomerRepository customerRepository, ILogger<UpdateCustomerStateCommandHandler> logger)
    : ICommandHandler<UpdateCustomerStateCommand, CustomerResponse>
{
    private readonly ILogger<UpdateCustomerStateCommandHandler> _logger = logger;

    public async Task<Result<CustomerResponse>> Handle(UpdateCustomerStateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateCustomerStateCommand));

        var existingCustomer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingCustomer is null)
        {
            return Result<CustomerResponse>.Failure(new Error("UpdateCustomerState.NotFound", "Customer not found"));
        }

        existingCustomer.State = request.State;

        await customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateCustomerStateCommand));
        return Result.Success(CustomerResponse.FromDomain(existingCustomer));
    }
}
