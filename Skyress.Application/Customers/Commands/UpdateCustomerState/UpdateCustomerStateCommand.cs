namespace Skyress.Application.Customers.Commands.UpdateCustomerState;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record UpdateCustomerStateCommand(
    long Id,
    CustomerState State,
    string LastEditedBy) : ICommand<Customer>;

public class UpdateCustomerStateCommandHandler(ICustomerRepository customerRepository)
    : ICommandHandler<UpdateCustomerStateCommand, Customer>
{
    public async Task<Result<Customer>> Handle(UpdateCustomerStateCommand request, CancellationToken cancellationToken)
    {
        var existingCustomer = await customerRepository.GetByIdAsync(request.Id);
        if (existingCustomer is null)
        {
            return Result<Customer>.Failure(new Error("UpdateCustomerState.NotFound", "Customer not found"));
        }

        existingCustomer.State = request.State;
        existingCustomer.LastEditDate = DateTime.UtcNow;
        existingCustomer.LastEditBy = request.LastEditedBy;
        
        await customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(existingCustomer);
    }
}