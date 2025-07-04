namespace Skyress.Application.Customers.Commands.UpdateCustomerNotes;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Common;

public record UpdateCustomerNotesCommand(
    long Id,
    string Notes) : ICommand<Customer>;

public class UpdateCustomerNotesCommandHandler(ICustomerRepository customerRepository)
    : ICommandHandler<UpdateCustomerNotesCommand, Customer>
{
    public async Task<Result<Customer>> Handle(UpdateCustomerNotesCommand request, CancellationToken cancellationToken)
    {
        var existingCustomer = await customerRepository.GetByIdAsync(request.Id);
        if (existingCustomer is null)
        {
            return Result<Customer>.Failure(new Error("UpdateCustomerNotes.NotFound", "Customer not found"));
        }

        existingCustomer.Notes = request.Notes;
        
        await customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(existingCustomer);
    }
}