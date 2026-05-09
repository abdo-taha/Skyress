namespace Skyress.Application.Invoices.Commands.UpdateInvoiceCustomerId;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Invoices.Responses;
using Skyress.Domain.Common;

public record UpdateInvoiceCustomerIdCommand(
    long Id,
    long CustomerId) : ICommand<InvoiceResponse>;

public class UpdateInvoiceCustomerIdCommandHandler : ICommandHandler<UpdateInvoiceCustomerIdCommand, InvoiceResponse>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<UpdateInvoiceCustomerIdCommandHandler> _logger;

    public UpdateInvoiceCustomerIdCommandHandler(IInvoiceRepository invoiceRepository, ILogger<UpdateInvoiceCustomerIdCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<Result<InvoiceResponse>> Handle(UpdateInvoiceCustomerIdCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateInvoiceCustomerIdCommand));

        var invoice = await _invoiceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice is null)
        {
            return Result<InvoiceResponse>.Failure(new Error("UpdateInvoiceCustomerId.NotFound", "Invoice not found"));
        }

        invoice.CustomerId = request.CustomerId;

        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateInvoiceCustomerIdCommand));
        return Result.Success(InvoiceResponse.FromDomain(invoice));
    }
}
