using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Tags.Responses;
using Skyress.Domain.Common;

namespace Skyress.Application.Tags.Queries.GetAllTags;

public record GetAllTagsQuery : IQuery<List<TagResponse>>;

public class GetAllTagsQueryHandler : IQueryHandler<GetAllTagsQuery, List<TagResponse>>
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<GetAllTagsQueryHandler> _logger;

    public GetAllTagsQueryHandler(ITagRepository tagRepository, ILogger<GetAllTagsQueryHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result<List<TagResponse>>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetAllTagsQuery));

        var tags = await _tagRepository.GetAllAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetAllTagsQuery), tags.Count);
        return Result.Success(tags.Select(TagResponse.FromDomain).ToList());
    }
}