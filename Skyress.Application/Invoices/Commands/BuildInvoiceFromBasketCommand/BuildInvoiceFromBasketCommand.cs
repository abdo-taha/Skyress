using MediatR;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Invoices.Commands.AddSoldItemToInvoice;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Invoices.Commands.BuildInvoiceFromBasketCommand;

public record BuildInvoiceFromBasketCommand(
    long InvoiceId,
    long BasketId) : ICommand;

public class BuildInvoiceFromBasketCommandHandler : ICommandHandler<BuildInvoiceFromBasketCommand>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMediator _mediator;

    public BuildInvoiceFromBasketCommandHandler(IBasketRepository basketRepository, IMediator mediator, IInvoiceRepository invoiceRepository)
    {
        _basketRepository = basketRepository;
        _mediator = mediator;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result> Handle(BuildInvoiceFromBasketCommand request, CancellationToken cancellationToken)
    {
        Basket? basket = await this._basketRepository.GetBasketWithItemsAsync(request.BasketId);
        if (basket == null)
        {
            throw new Exception();
        }
        await AddSoldItemToInvoice(basket, request.InvoiceId);
        await UpdateInvoiceStatus(request.InvoiceId);

        return Result.Success();
    }

    private async Task AddSoldItemToInvoice(Basket basket, long invoiceId)
    {
        foreach (BasketItem basketBasketItem in basket.BasketItems)
        {
            AddSoldItemToInvoiceCommand command = new AddSoldItemToInvoiceCommand(invoiceId,
                basketBasketItem.ItemId, basketBasketItem.Quantity);
            await this._mediator.Send(command);
        }
    }

    private async Task UpdateInvoiceStatus(long invoiceId)
    {
        Invoice? invoice = await this._invoiceRepository.GetByIdAsync(invoiceId);
        if (invoice == null)
        {
            throw new Exception();
        }
        invoice.State = InvoiceState.Issued;
        await this._invoiceRepository.UnitOfWork.SaveChangesAsync();
    }
} 