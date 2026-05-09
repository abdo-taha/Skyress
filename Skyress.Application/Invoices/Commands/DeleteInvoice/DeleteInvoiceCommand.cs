namespace Skyress.Application.Invoices.Commands.DeleteInvoice;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

public record DeleteInvoiceCommand(long Id) : ICommand;

public class DeleteInvoiceCommandHandler : ICommandHandler<DeleteInvoiceCommand>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<DeleteInvoiceCommandHandler> _logger;

    public DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepository, ILogger<DeleteInvoiceCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command} for InvoiceId: {Id}", nameof(DeleteInvoiceCommand), request.Id);

        await _invoiceRepository.DeleteByIdAsync(request.Id, cancellationToken);
        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. InvoiceId: {Id}", nameof(DeleteInvoiceCommand), request.Id);
        return Result.Success();
    }
}
