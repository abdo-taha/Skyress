using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

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

    public AddSoldItemToInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IItemRepository itemRepository)
    {
        _invoiceRepository = invoiceRepository;
        _itemRepository = itemRepository;
    }

    public async Task<Result<SoldItem>> Handle(AddSoldItemToInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
        
        if (invoice is null)
        {
            return Result<SoldItem>.Failure(new Error("Invoice.NotFound", "Invoice not found"));
        }
        
        var item = await _itemRepository.GetByIdAsync(request.ItemId);
        
        if (item is null)
        {
            return Result<SoldItem>.Failure(Error.Dummy);
        }

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

        invoice.AddSoldItem(soldItem);
        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success(soldItem);
    }
} 