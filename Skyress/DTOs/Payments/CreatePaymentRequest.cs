using Skyress.Domain.Enums;

namespace Skyress.API.DTOs.Payments;

public record CreatePaymentRequest(
    long InvoiceId,
    PaymentType PaymentType);