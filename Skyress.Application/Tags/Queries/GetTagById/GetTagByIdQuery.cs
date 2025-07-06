using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Common;

namespace Skyress.Application.Tags.Queries.GetTagById;

public record GetTagByIdQuery(long Id) : IQuery<Tag>;

public class GetTagByIdQueryHandler : IQueryHandler<GetTagByIdQuery, Tag>
{
    private readonly ITagRepository _tagRepository;

    public GetTagByIdQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<Tag>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(request.Id);

        if (tag is null || tag.IsDeleted)
        {
            return Result<Tag>.Failure(new Error("", ""));
        }

        return Result.Success(tag);
    }
}