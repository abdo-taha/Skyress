using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.TagAssignments.Commands.DeleteTagAssignment;

public record DeleteTagAssignmentCommand(long Id) : ICommand;

public class DeleteTagAssignmentCommandHandler : ICommandHandler<DeleteTagAssignmentCommand>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;

    public DeleteTagAssignmentCommandHandler(ITagAssignmentRepository tagAssignmentRepository)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
    }

    public async Task<Result> Handle(DeleteTagAssignmentCommand request, CancellationToken cancellationToken)
    {
        var tagAssignment = await _tagAssignmentRepository.GetByIdAsync(request.Id);

        if (tagAssignment is null)
        {
            return Result.Failure(new Error("",""));
        }

        await _tagAssignmentRepository.DeleteByIdAsync(request.Id);
        await _tagAssignmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}