namespace Skyress.API.DTOs.Customers;

public record UpdateCustomerNotesRequest(
    long Id,
    string Notes,
    string? EditedBy = null
);