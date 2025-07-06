namespace Skyress.Application.Invoices.Commands.DeleteInvoice;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

public record DeleteInvoiceCommand(long Id) : ICommand;

public class DeleteInvoiceCommandHandler : ICommandHandler<DeleteInvoiceCommand>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        await _invoiceRepository.DeleteByIdAsync(request.Id);
        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}