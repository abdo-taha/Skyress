namespace Skyress.Application.Invoices.Queries.GetInvoiceById;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;

public record GetInvoiceByIdQuery(long Id) : IQuery<Invoice>;

public class GetInvoiceByIdQueryHandler : IQueryHandler<GetInvoiceByIdQuery, Invoice>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<Invoice>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);
        if (invoice is null)
        {
            return Result<Invoice>.Failure(new Error("GetInvoiceById.NotFound", "Invoice not found"));
        }

        return Result.Success(invoice);
    }
}