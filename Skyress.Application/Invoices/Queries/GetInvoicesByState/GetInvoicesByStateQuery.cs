using Microsoft.EntityFrameworkCore;

namespace Skyress.Application.Invoices.Queries.GetInvoicesByState;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record GetInvoicesByStateQuery(InvoiceState State) : IQuery<List<Invoice>>;

public class GetInvoicesByStateQueryHandler : IQueryHandler<GetInvoicesByStateQuery, List<Invoice>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoicesByStateQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<List<Invoice>>> Handle(GetInvoicesByStateQuery request, CancellationToken cancellationToken)
    {
        var invoices = _invoiceRepository.GetAsync(
            predicate: i => i.State == request.State,
            disableTracking: true);
            
        return Result.Success(await invoices.ToListAsync(cancellationToken));
    }
}