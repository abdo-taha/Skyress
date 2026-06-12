using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;
using Skyress.Domain.Exceptions;

namespace Skyress.Application.Invoices.Commands.AddSoldItemToInvoice;

public record AddSoldItemToInvoiceCommand(
    long InvoiceId,
    long ItemId,
    int Quantity,
    TransactionType TransactionType = TransactionType.Sell) : ICommand<SoldItem>;

public class AddSoldItemToInvoiceCommandHandler : ICommandHandler<AddSoldItemToInvoiceCommand, SoldItem>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<AddSoldItemToInvoiceCommandHandler> _logger;

    public AddSoldItemToInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IItemRepository itemRepository, ILogger<AddSoldItemToInvoiceCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<Result<SoldItem>> Handle(AddSoldItemToInvoiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(AddSoldItemToInvoiceCommand));

        // Load invoice WITH its sold items so we can check for duplicates
        var invoice = await _invoiceRepository.GetByIdWithSoldItemsAsync(request.InvoiceId, cancellationToken);

        if (invoice is null)
            return Result<SoldItem>.Failure(new Error("Invoice.NotFound", "Invoice not found"));

        // Idempotency: skip if a sold item for this item already exists in the invoice
        var existingSoldItem = invoice.SoldItems.FirstOrDefault(si => si.ItemId == request.ItemId);
        if (existingSoldItem is not null)
        {
            _logger.LogInformation("{Command} skipped — SoldItem already exists for ItemId {ItemId}",
                nameof(AddSoldItemToInvoiceCommand), request.ItemId);
            return Result.Success(existingSoldItem);
        }

        var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
        if (item is null)
            return Result<SoldItem>.Failure(Error.Dummy);

        var soldItem = new SoldItem
        {
            Name = item.Name,
            Price = item.Price,
            Quantity = request.Quantity,
            TransactionType = request.TransactionType,
            SellingTime = DateTime.UtcNow,
            InvoiceId = request.InvoiceId,
            ItemId = request.ItemId,
            ItemCost = item.CostPrice,
        };

        try
        {
            invoice.AddSoldItem(soldItem);
        }
        catch (DomainException exception)
        {
            return DomainExceptionResultMapper.ToFailure<SoldItem>(exception);
        }

        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(AddSoldItemToInvoiceCommand));
        return Result.Success(soldItem);
    }
}
