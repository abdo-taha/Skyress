namespace Skyress.Application.Customers.Commands.DeleteCustomer;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

public record DeleteCustomerCommand(long Id) : ICommand;

public class DeleteCustomerCommandHandler(ICustomerRepository customerRepository, ILogger<DeleteCustomerCommandHandler> logger)
    : ICommandHandler<DeleteCustomerCommand>
{
    private readonly ILogger<DeleteCustomerCommandHandler> _logger = logger;

    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command} for CustomerId: {Id}", nameof(DeleteCustomerCommand), request.Id);

        await customerRepository.DeleteByIdAsync(request.Id, cancellationToken);
        await customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. CustomerId: {Id}", nameof(DeleteCustomerCommand), request.Id);
        return Result.Success();
    }
}
