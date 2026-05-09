using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Skyress.Application.Invoices.Queries.GetInvoicesByCustomer;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Invoices.Responses;
using Skyress.Domain.Common;

public record GetInvoicesByCustomerQuery(long CustomerId) : IQuery<List<InvoiceResponse>>;

public class GetInvoicesByCustomerQueryHandler : IQueryHandler<GetInvoicesByCustomerQuery, List<InvoiceResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<GetInvoicesByCustomerQueryHandler> _logger;

    public GetInvoicesByCustomerQueryHandler(IInvoiceRepository invoiceRepository, ILogger<GetInvoicesByCustomerQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<Result<List<InvoiceResponse>>> Handle(GetInvoicesByCustomerQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetInvoicesByCustomerQuery));

        var invoices = _invoiceRepository.GetAsync(
            predicate: i => i.CustomerId == request.CustomerId,
            disableTracking: true);

        var result = await invoices.ToListAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetInvoicesByCustomerQuery), result.Count);
        return Result.Success(result.Select(InvoiceResponse.FromDomain).ToList());
    }
}
