using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.TagAssignmnet;
using Skyress.Domain.Common;

namespace Skyress.Application.TagAssignments.Commands.CreateTagAssignment;

public record CreateTagAssignmentCommand(long TagId, long ItemId) : ICommand<TagAssignment>;

public class CreateTagAssignmentCommandHandler : ICommandHandler<CreateTagAssignmentCommand, TagAssignment>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly ITagRepository _tagRepository;

    public CreateTagAssignmentCommandHandler(
        ITagAssignmentRepository tagAssignmentRepository,
        ITagRepository tagRepository)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
        _tagRepository = tagRepository;
    }

    public async Task<Result<TagAssignment>> Handle(CreateTagAssignmentCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(request.TagId);
        if (tag is null)
        {
            return Result<TagAssignment>.Failure(new Error(" ",""));
        }

        var tagAssignment = new TagAssignment
        {
            TagId = request.TagId,
            ItemId = request.ItemId
        };

        await _tagAssignmentRepository.CreateAsync(tagAssignment);
        await _tagAssignmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(tagAssignment);
    }
}