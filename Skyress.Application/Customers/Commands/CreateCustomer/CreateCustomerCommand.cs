namespace Skyress.Application.Customers.Commands.CreateCustomer;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record CreateCustomerCommand(
    string Name,
    string Notes,
    CustomerState State) : ICommand<Customer>;

public class CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    : ICommandHandler<CreateCustomerCommand, Customer>
{
    public async Task<Result<Customer>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Name = request.Name,
            Notes = request.Notes,
            State = request.State
        };

        var createdCustomer = await customerRepository.CreateAsync(customer);
        await customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(createdCustomer);
    }
}