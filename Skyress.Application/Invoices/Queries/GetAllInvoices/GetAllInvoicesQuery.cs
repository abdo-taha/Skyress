namespace Skyress.Application.Invoices.Queries.GetAllInvoices;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;

public record GetAllInvoicesQuery() : IQuery<List<Invoice>>;

public class GetAllInvoicesQueryHandler : IQueryHandler<GetAllInvoicesQuery, List<Invoice>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetAllInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<List<Invoice>>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetAllAsync();
        return Result.Success(invoices.ToList());
    }
}