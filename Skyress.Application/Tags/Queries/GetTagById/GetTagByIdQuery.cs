using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Tags.Responses;
using Skyress.Domain.Common;

namespace Skyress.Application.Tags.Queries.GetTagById;

public record GetTagByIdQuery(long Id) : IQuery<TagResponse>;

public class GetTagByIdQueryHandler : IQueryHandler<GetTagByIdQuery, TagResponse>
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<GetTagByIdQueryHandler> _logger;

    public GetTagByIdQueryHandler(ITagRepository tagRepository, ILogger<GetTagByIdQueryHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result<TagResponse>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(GetTagByIdQuery));

        var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);

        if (tag is null || tag.IsDeleted)
        {
            return Result<TagResponse>.Failure(new Error("", ""));
        }

        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(GetTagByIdQuery), tag.Id);
        return Result.Success(TagResponse.FromDomain(tag));
    }
}