using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Baskets.Commands.DeleteBasketCommand;

public class DeleteBasketCommandHandler(IBasketRepository basketRepository): ICommandHandler<DeleteBasketCommand>
{
    public async Task<Result> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
    {
        await basketRepository.DeleteByIdAsync(request.BasketId);

        await basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}