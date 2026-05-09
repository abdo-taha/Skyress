namespace Skyress.Application.Invoices.Queries.GetInvoiceById;

using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Invoices.Responses;
using Skyress.Domain.Common;

public record GetInvoiceByIdQuery(long Id) : IQuery<InvoiceResponse>;

public class GetInvoiceByIdQueryHandler : IQueryHandler<GetInvoiceByIdQuery, InvoiceResponse>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<GetInvoiceByIdQueryHandler> _logger;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository, ILogger<GetInvoiceByIdQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<Result<InvoiceResponse>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetInvoiceByIdQuery));

        var invoice = await _invoiceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice is null)
        {
            return Result<InvoiceResponse>.Failure(new Error("GetInvoiceById.NotFound", "Invoice not found"));
        }

        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(GetInvoiceByIdQuery), invoice.Id);
        return Result.Success(InvoiceResponse.FromDomain(invoice));
    }
}
