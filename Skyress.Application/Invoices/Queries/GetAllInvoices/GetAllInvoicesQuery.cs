namespace Skyress.Application.Invoices.Queries.GetAllInvoices;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Invoices.Responses;
using Skyress.Domain.Common;

public record GetAllInvoicesQuery() : IQuery<IReadOnlyList<InvoiceResponse>>;

public class GetAllInvoicesQueryHandler : IQueryHandler<GetAllInvoicesQuery, IReadOnlyList<InvoiceResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<GetAllInvoicesQueryHandler> _logger;

    public GetAllInvoicesQueryHandler(IInvoiceRepository invoiceRepository, ILogger<GetAllInvoicesQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<InvoiceResponse>>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetAllInvoicesQuery));

        var invoices = await _invoiceRepository.GetAllAsync(cancellationToken);
        var response = invoices.Select(InvoiceResponse.FromDomain).ToList().AsReadOnly();
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetAllInvoicesQuery), response.Count);
        return Result.Success<IReadOnlyList<InvoiceResponse>>(response);
    }
}
