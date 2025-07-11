using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.CreateBasket;

public record CreateBasketCommand(long CustomerId) : ICommand<Basket>;

public class CreateBasketCommandHandler(IBasketRepository basketRepository, ICustomerRepository customerRepository)
    : ICommandHandler<CreateBasketCommand, Basket>
{

    public async Task<Result<Basket>> Handle(CreateBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = new Basket
        {
            UserId = request.CustomerId,
        };
        var customer = await customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            return Result<Basket>.Failure(Error.Dummy);
        }
        await basketRepository.CreateAsync(basket);
        
        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(basket);
    }
}