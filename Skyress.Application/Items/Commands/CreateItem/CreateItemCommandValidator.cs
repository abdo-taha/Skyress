namespace Skyress.Application.Items.Commands.CreateItem;

using FluentValidation;

public sealed class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.QuantityLeft).GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative.");
    }
}
