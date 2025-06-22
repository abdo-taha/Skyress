using Skyress.Domain.Enums;

namespace Skyress.API.DTOs.Customers;

public record CreateCustomerRequest(
    string Name,
    string Notes,
    CustomerState State = CustomerState.Active
);