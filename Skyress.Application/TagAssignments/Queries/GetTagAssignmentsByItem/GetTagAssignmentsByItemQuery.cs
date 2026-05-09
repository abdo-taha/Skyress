using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.TagAssignments.Responses;
using Skyress.Domain.Common;

namespace Skyress.Application.TagAssignments.Queries.GetTagAssignmentsByItem;

public record GetTagAssignmentsByItemQuery(long ItemId) : IQuery<List<TagAssignmentResponse>>;

public class GetTagAssignmentsByItemQueryHandler : IQueryHandler<GetTagAssignmentsByItemQuery, List<TagAssignmentResponse>>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly ILogger<GetTagAssignmentsByItemQueryHandler> _logger;

    public GetTagAssignmentsByItemQueryHandler(ITagAssignmentRepository tagAssignmentRepository, ILogger<GetTagAssignmentsByItemQueryHandler> logger)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
        _logger = logger;
    }

    public async Task<Result<List<TagAssignmentResponse>>> Handle(GetTagAssignmentsByItemQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetTagAssignmentsByItemQuery));

        var tagAssignments = await _tagAssignmentRepository.GetAsync(
            predicate: ta => ta.ItemId == request.ItemId, disableTracking: true).ToListAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetTagAssignmentsByItemQuery), tagAssignments.Count);
        return Result.Success(tagAssignments.Select(TagAssignmentResponse.FromDomain).ToList());
    }
}