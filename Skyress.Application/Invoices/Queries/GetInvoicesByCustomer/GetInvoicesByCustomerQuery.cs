using Microsoft.EntityFrameworkCore;

namespace Skyress.Application.Invoices.Queries.GetInvoicesByCustomer;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;

public record GetInvoicesByCustomerQuery(long CustomerId) : IQuery<List<Invoice>>;

public class GetInvoicesByCustomerQueryHandler : IQueryHandler<GetInvoicesByCustomerQuery, List<Invoice>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoicesByCustomerQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

        public async Task<Result<List<Invoice>>> Handle(GetInvoicesByCustomerQuery request, CancellationToken cancellationToken)
        {
            var invoices = _invoiceRepository.GetAsync(
                predicate: i => i.CustomerId == request.CustomerId,
                disableTracking: true);
                
            return Result.Success(await invoices.ToListAsync(cancellationToken));
        }
}