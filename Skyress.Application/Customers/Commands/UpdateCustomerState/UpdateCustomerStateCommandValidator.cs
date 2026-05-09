namespace Skyress.Application.Customers.Commands.UpdateCustomerState;

using FluentValidation;

public sealed class UpdateCustomerStateCommandValidator : AbstractValidator<UpdateCustomerStateCommand>
{
    public UpdateCustomerStateCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Customer ID must be valid.");
        RuleFor(x => x.State).IsInEnum().WithMessage("State must be a valid value.");
    }
}
