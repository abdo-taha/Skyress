namespace Skyress.Application.Invoices.Commands.UpdateInvoiceCustomerId;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;

public record UpdateInvoiceCustomerIdCommand(
    long Id,
    long CustomerId) : ICommand<Invoice>;

public class UpdateInvoiceCustomerIdCommandHandler : ICommandHandler<UpdateInvoiceCustomerIdCommand, Invoice>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public UpdateInvoiceCustomerIdCommandHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<Invoice>> Handle(UpdateInvoiceCustomerIdCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);
        if (invoice is null)
        {
            return Result<Invoice>.Failure(new Error("UpdateInvoiceCustomerId.NotFound", "Invoice not found"));
        }

        invoice.CustomerId = request.CustomerId;
        
        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success(invoice);
    }
}