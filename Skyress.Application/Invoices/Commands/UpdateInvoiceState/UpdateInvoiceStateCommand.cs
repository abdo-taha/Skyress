namespace Skyress.Application.Invoices.Commands.UpdateInvoiceState;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record UpdateInvoiceStateCommand(
    long Id,
    InvoiceState State) : ICommand<Invoice>;

public class UpdateInvoiceStateCommandHandler : ICommandHandler<UpdateInvoiceStateCommand, Invoice>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public UpdateInvoiceStateCommandHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<Invoice>> Handle(UpdateInvoiceStateCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);
        if (invoice is null)
        {
            return Result<Invoice>.Failure(new Error("UpdateInvoiceState.NotFound", "Invoice not found"));
        }

        invoice.State = request.State;
        
        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success(invoice);
    }
}