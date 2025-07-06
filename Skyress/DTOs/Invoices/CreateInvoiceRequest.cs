namespace Skyress.API.DTOs.Invoices;

public record CreateInvoiceRequest(long? CustomerId, decimal TotalAmount);