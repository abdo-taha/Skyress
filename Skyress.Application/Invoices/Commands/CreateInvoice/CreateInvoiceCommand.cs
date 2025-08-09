namespace Skyress.Application.Invoices.Commands.CreateInvoice;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record CreateInvoiceCommand(
    long? CustomerId,
    InvoiceState State = InvoiceState.Issued) : ICommand<Invoice>;

public class CreateInvoiceCommandHandler : ICommandHandler<CreateInvoiceCommand, Invoice>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = new Invoice
        {
            CustomerId = request.CustomerId,
            TotalAmount = 0,
            State = request.State,
        };

        var createdInvoice = await _invoiceRepository.CreateAsync(invoice);
        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success(createdInvoice);
    }
}