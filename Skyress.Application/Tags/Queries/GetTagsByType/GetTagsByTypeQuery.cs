using MediatR;
using Microsoft.Extensions.Logging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Tags.Responses;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Tags.Queries.GetTagsByType;

public record GetTagsByTypeQuery(TagType Type) : IRequest<Result<List<TagResponse>>>;

public class GetTagsByTypeQueryHandler : IRequestHandler<GetTagsByTypeQuery, Result<List<TagResponse>>>
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<GetTagsByTypeQueryHandler> _logger;

    public GetTagsByTypeQueryHandler(ITagRepository tagRepository, ILogger<GetTagsByTypeQueryHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result<List<TagResponse>>> Handle(GetTagsByTypeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetTagsByTypeQuery));

        var tags = await _tagRepository.GetTagsByTypeAsync(request.Type, cancellationToken);
        _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetTagsByTypeQuery), tags.Count);
        return Result.Success(tags.Select(TagResponse.FromDomain).ToList());
    }
}