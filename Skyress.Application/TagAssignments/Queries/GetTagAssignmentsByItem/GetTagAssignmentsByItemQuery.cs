using Microsoft.EntityFrameworkCore;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.TagAssignmnet;
using Skyress.Domain.Common;

namespace Skyress.Application.TagAssignments.Queries.GetTagAssignmentsByItem;

public record GetTagAssignmentsByItemQuery(long ItemId) : IQuery<List<TagAssignment>>;

public class GetTagAssignmentsByItemQueryHandler : IQueryHandler<GetTagAssignmentsByItemQuery, List<TagAssignment>>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;

    public GetTagAssignmentsByItemQueryHandler(ITagAssignmentRepository tagAssignmentRepository)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
    }

    public async Task<Result<List<TagAssignment>>> Handle(GetTagAssignmentsByItemQuery request, CancellationToken cancellationToken)
    {
        var tagAssignments = await _tagAssignmentRepository.GetAsync(
            predicate: ta => ta.ItemId == request.ItemId, disableTracking: true).ToListAsync(cancellationToken: cancellationToken);

        return Result.Success(tagAssignments);
    }
}