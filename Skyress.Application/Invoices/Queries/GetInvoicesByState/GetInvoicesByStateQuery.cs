using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Skyress.Application.Invoices.Queries.GetInvoicesByState;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Invoices.Responses;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

public record GetInvoicesByStateQuery(InvoiceState State) : IQuery<List<InvoiceResponse>>;

public class GetInvoicesByStateQueryHandler : IQueryHandler<GetInvoicesByStateQuery, List<InvoiceResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<GetInvoicesByStateQueryHandler> _logger;

    public GetInvoicesByStateQueryHandler(IInvoiceRepository invoiceRepository, ILogger<GetInvoicesByStateQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<Result<List<InvoiceResponse>>> Handle(GetInvoicesByStateQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetInvoicesByStateQuery));

        var invoices = _invoiceRepository.GetAsync(
            predicate: i => i.State == request.State,
            disableTracking: true);

        var result = await invoices.ToListAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetInvoicesByStateQuery), result.Count);
        return Result.Success(result.Select(InvoiceResponse.FromDomain).ToList());
    }
}
