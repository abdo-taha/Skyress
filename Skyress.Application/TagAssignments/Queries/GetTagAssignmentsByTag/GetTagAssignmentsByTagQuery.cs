using Microsoft.EntityFrameworkCore;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.TagAssignmnet;
using Skyress.Domain.Common;

namespace Skyress.Application.TagAssignments.Queries.GetTagAssignmentsByTag;

public record GetTagAssignmentsByTagQuery(long TagId) : IQuery<List<TagAssignment>>;

public class GetTagAssignmentsByTagQueryHandler : IQueryHandler<GetTagAssignmentsByTagQuery, List<TagAssignment>>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;

    public GetTagAssignmentsByTagQueryHandler(ITagAssignmentRepository tagAssignmentRepository)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
    }

    public async Task<Result<List<TagAssignment>>> Handle(GetTagAssignmentsByTagQuery request, CancellationToken cancellationToken)
    {
        var tagAssignments = await _tagAssignmentRepository.GetAsync(
            predicate: ta => ta.TagId == request.TagId, disableTracking: true).ToListAsync(cancellationToken: cancellationToken);

        return Result.Success(tagAssignments);
    }
}