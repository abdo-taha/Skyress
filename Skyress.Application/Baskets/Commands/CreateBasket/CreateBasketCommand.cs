using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.CreateBasket;

public record CreateBasketCommand(long? CustomerId) : ICommand<Basket>;

public class CreateBasketCommandHandler(IBasketRepository basketRepository)
    : ICommandHandler<CreateBasketCommand, Basket>
{

    public async Task<Result<Basket>> Handle(CreateBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = new Basket
        {
            UserId = request.CustomerId,
        };
        await basketRepository.CreateAsync(basket);
        
        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(basket);
    }
}