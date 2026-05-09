using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.TagAssignments.Responses;
using Skyress.Domain.Common;

namespace Skyress.Application.TagAssignments.Queries.GetTagAssignmentsByTag;

public record GetTagAssignmentsByTagQuery(long TagId) : IQuery<List<TagAssignmentResponse>>;

public class GetTagAssignmentsByTagQueryHandler : IQueryHandler<GetTagAssignmentsByTagQuery, List<TagAssignmentResponse>>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly ILogger<GetTagAssignmentsByTagQueryHandler> _logger;

    public GetTagAssignmentsByTagQueryHandler(ITagAssignmentRepository tagAssignmentRepository, ILogger<GetTagAssignmentsByTagQueryHandler> logger)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
        _logger = logger;
    }

    public async Task<Result<List<TagAssignmentResponse>>> Handle(GetTagAssignmentsByTagQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetTagAssignmentsByTagQuery));

        var tagAssignments = await _tagAssignmentRepository.GetAsync(
            predicate: ta => ta.TagId == request.TagId, disableTracking: true).ToListAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetTagAssignmentsByTagQuery), tagAssignments.Count);
        return Result.Success(tagAssignments.Select(TagAssignmentResponse.FromDomain).ToList());
    }
}