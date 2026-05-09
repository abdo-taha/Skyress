namespace Skyress.Application.Customers.Responses;

using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Enums;

public sealed record CustomerResponse(
    long Id,
    string Name,
    string Notes,
    CustomerState State,
    DateTime CreatedAt)
{
    public static CustomerResponse FromDomain(Customer customer) => new(
        customer.Id,
        customer.Name,
        customer.Notes,
        customer.State,
        customer.CreatedAt);
}
