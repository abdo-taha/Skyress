using MediatR;
using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Invoices.Commands.AddSoldItemToInvoice;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;
using Skyress.Domain.Exceptions;

namespace Skyress.Application.Invoices.Commands.BuildInvoiceFromBasketCommand;

public record BuildInvoiceFromBasketCommand(
    long InvoiceId,
    long BasketId) : ICommand;

public class BuildInvoiceFromBasketCommandHandler : ICommandHandler<BuildInvoiceFromBasketCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<BuildInvoiceFromBasketCommandHandler> _logger;

    public BuildInvoiceFromBasketCommandHandler(IBasketRepository basketRepository, IMediator mediator, IInvoiceRepository invoiceRepository, ILogger<BuildInvoiceFromBasketCommandHandler> logger)
    {
        _basketRepository = basketRepository;
        _mediator = mediator;
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(BuildInvoiceFromBasketCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(BuildInvoiceFromBasketCommand));

        Basket? basket = await _basketRepository.GetBasketWithItemsAsync(request.BasketId);
        if (basket == null)
        {
            throw new Exception();
        }
        var soldItemsResult = await AddSoldItemToInvoice(basket, request.InvoiceId, cancellationToken);
        if (soldItemsResult.IsFailure)
        {
            return soldItemsResult;
        }

        var invoiceStatusResult = await UpdateInvoiceStatus(request.InvoiceId, cancellationToken);
        if (invoiceStatusResult.IsFailure)
        {
            return invoiceStatusResult;
        }

        _logger.LogInformation("{Command} completed", nameof(BuildInvoiceFromBasketCommand));
        return Result.Success();
    }

    private async Task<Result> AddSoldItemToInvoice(Basket basket, long invoiceId, CancellationToken cancellationToken)
    {
        foreach (BasketItem basketBasketItem in basket.BasketItems)
        {
            AddSoldItemToInvoiceCommand command = new AddSoldItemToInvoiceCommand(invoiceId,
                basketBasketItem.ItemId, basketBasketItem.Quantity);
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
        }

        return Result.Success();
    }

    private async Task<Result> UpdateInvoiceStatus(long invoiceId, CancellationToken cancellationToken)
    {
        Invoice? invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
        if (invoice == null)
            return Result.Failure(new Error("Invoice.NotFound", "Invoice not found"));

        // Idempotency: skip if already issued or beyond
        if (invoice.State >= InvoiceState.Issued)
            return Result.Success();

        try
        {
            invoice.Issue();
        }
        catch (DomainException exception)
        {
            return DomainExceptionResultMapper.ToFailure(exception);
        }

        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
