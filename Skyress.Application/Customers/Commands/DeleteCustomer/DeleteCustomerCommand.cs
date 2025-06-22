namespace Skyress.Application.Customers.Commands.DeleteCustomer;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

public record DeleteCustomerCommand(long Id) : ICommand;

public class DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    : ICommandHandler<DeleteCustomerCommand>
{
    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        await customerRepository.DeleteByIdAsync(request.Id);
        
        await customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}