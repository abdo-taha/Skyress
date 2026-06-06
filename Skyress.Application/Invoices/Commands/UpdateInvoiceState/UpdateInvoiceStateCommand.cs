namespace Skyress.Application.Invoices.Commands.UpdateInvoiceState;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Invoices.Responses;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record UpdateInvoiceStateCommand(
    long Id,
    InvoiceState State) : ICommand<InvoiceResponse>;

public class UpdateInvoiceStateCommandHandler : ICommandHandler<UpdateInvoiceStateCommand, InvoiceResponse>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<UpdateInvoiceStateCommandHandler> _logger;

    public UpdateInvoiceStateCommandHandler(IInvoiceRepository invoiceRepository, ILogger<UpdateInvoiceStateCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<Result<InvoiceResponse>> Handle(UpdateInvoiceStateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateInvoiceStateCommand));

        var invoice = await _invoiceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice is null)
            return Result<InvoiceResponse>.Failure(new Error("UpdateInvoiceState.NotFound", "Invoice not found"));

        // Idempotency: no-op if invoice is already in the target state
        if (invoice.State == request.State)
        {
            _logger.LogInformation("{Command} skipped — invoice already in state {State}",
                nameof(UpdateInvoiceStateCommand), request.State);
            return Result.Success(InvoiceResponse.FromDomain(invoice));
        }

        invoice.State = request.State;

        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateInvoiceStateCommand));
        return Result.Success(InvoiceResponse.FromDomain(invoice));
    }
}
