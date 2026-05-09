namespace Skyress.Application.Customers.Commands.UpdateCustomerNotes;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Customers.Responses;
using Skyress.Domain.Common;

public record UpdateCustomerNotesCommand(
    long Id,
    string Notes) : ICommand<CustomerResponse>;

public class UpdateCustomerNotesCommandHandler(ICustomerRepository customerRepository, ILogger<UpdateCustomerNotesCommandHandler> logger)
    : ICommandHandler<UpdateCustomerNotesCommand, CustomerResponse>
{
    private readonly ILogger<UpdateCustomerNotesCommandHandler> _logger = logger;

    public async Task<Result<CustomerResponse>> Handle(UpdateCustomerNotesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateCustomerNotesCommand));

        var existingCustomer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingCustomer is null)
        {
            return Result<CustomerResponse>.Failure(new Error("UpdateCustomerNotes.NotFound", "Customer not found"));
        }

        existingCustomer.Notes = request.Notes;

        await customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateCustomerNotesCommand));
        return Result.Success(CustomerResponse.FromDomain(existingCustomer));
    }
}
