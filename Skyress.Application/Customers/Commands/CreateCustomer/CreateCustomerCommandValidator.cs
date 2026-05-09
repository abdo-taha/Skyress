namespace Skyress.Application.Customers.Commands.CreateCustomer;

using FluentValidation;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Notes).NotEmpty().WithMessage("Notes are required.");
        RuleFor(x => x.State).IsInEnum().WithMessage("State must be a valid value.");
    }
}
