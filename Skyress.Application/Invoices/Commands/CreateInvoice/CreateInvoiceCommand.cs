namespace Skyress.Application.Invoices.Commands.CreateInvoice;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Invoices.Responses;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record CreateInvoiceCommand(
    long BasketId,
    InvoiceState State = InvoiceState.Draft) : ICommand<InvoiceResponse>;

public class CreateInvoiceCommandHandler : ICommandHandler<CreateInvoiceCommand, InvoiceResponse>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;

    public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IBasketRepository basketRepository, ILogger<CreateInvoiceCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _basketRepository = basketRepository;
        _logger = logger;
    }

    public async Task<Result<InvoiceResponse>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(CreateInvoiceCommand));

        // Idempotency: return existing invoice if one already exists for this basket
        var existing = await _invoiceRepository.GetByBasketIdAsync(request.BasketId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("{Command} skipped — invoice already exists. Id: {Id}", nameof(CreateInvoiceCommand), existing.Id);
            return Result.Success(InvoiceResponse.FromDomain(existing));
        }

        var basket = await _basketRepository.GetByIdAsync(request.BasketId, cancellationToken);
        var invoice = new Invoice
        {
            BasketId = request.BasketId,
            CustomerId = basket?.UserId,
            TotalAmount = 0,
            State = request.State,
        };

        var createdInvoice = await _invoiceRepository.CreateAsync(invoice, cancellationToken);
        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(CreateInvoiceCommand), createdInvoice.Id);
        return Result.Success(InvoiceResponse.FromDomain(createdInvoice));
    }
}
