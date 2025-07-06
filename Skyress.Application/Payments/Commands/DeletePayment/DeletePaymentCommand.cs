namespace Skyress.Application.Payments.Commands.DeletePayment;

using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

public record DeletePaymentCommand(long Id) : ICommand;

public class DeletePaymentCommandHandler : ICommandHandler<DeletePaymentCommand>
{
    private readonly IPaymentRepository _paymentRepository;

    public DeletePaymentCommandHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        await _paymentRepository.DeleteByIdAsync(request.Id);
        await _paymentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}