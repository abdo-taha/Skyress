using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Common;

namespace Skyress.Application.Tags.Queries.GetAllTags;

public record GetAllTagsQuery : IQuery<List<Tag>>;

public class GetAllTagsQueryHandler : IQueryHandler<GetAllTagsQuery, List<Tag>>
{
    private readonly ITagRepository _tagRepository;

    public GetAllTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<List<Tag>>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAllAsync();
        return Result.Success(tags.ToList());
    }
}