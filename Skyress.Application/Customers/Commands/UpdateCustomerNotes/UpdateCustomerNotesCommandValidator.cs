namespace Skyress.Application.Customers.Commands.UpdateCustomerNotes;

using FluentValidation;

public sealed class UpdateCustomerNotesCommandValidator : AbstractValidator<UpdateCustomerNotesCommand>
{
    public UpdateCustomerNotesCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Customer ID must be valid.");
        RuleFor(x => x.Notes).NotEmpty().WithMessage("Notes are required.");
    }
}
