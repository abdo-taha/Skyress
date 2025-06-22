using Skyress.Domain.Enums;

namespace Skyress.API.DTOs.Customers;

public record UpdateCustomerStateRequest(
    long Id,
    CustomerState State,
    string LastEditedBy
);