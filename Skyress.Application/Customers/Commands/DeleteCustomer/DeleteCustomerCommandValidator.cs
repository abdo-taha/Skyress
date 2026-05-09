namespace Skyress.Application.Customers.Commands.DeleteCustomer;

using FluentValidation;

public sealed class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Customer ID must be valid.");
    }
}
