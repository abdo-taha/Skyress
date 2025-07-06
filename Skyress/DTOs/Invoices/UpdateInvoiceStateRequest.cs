using Skyress.Domain.Enums;

namespace Skyress.API.DTOs.Invoices;

public record UpdateInvoiceStateRequest(InvoiceState State);