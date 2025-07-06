using MediatR;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Tags.Queries.GetTagsByType;

public record GetTagsByTypeQuery(TagType Type) : IRequest<Result<List<Tag>>>;

public class GetTagsByTypeQueryHandler : IRequestHandler<GetTagsByTypeQuery, Result<List<Tag>>>
{
    private readonly ITagRepository _tagRepository;

    public GetTagsByTypeQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<List<Tag>>> Handle(GetTagsByTypeQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetTagsByTypeAsync(request.Type, cancellationToken);
        return Result.Success(tags);
    }
}