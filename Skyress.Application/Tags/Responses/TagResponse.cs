namespace Skyress.Application.Tags.Responses;

using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Enums;

public sealed record TagResponse(
    long Id,
    string Name,
    TagType Type)
{
    public static TagResponse FromDomain(Tag tag) => new(
        tag.Id,
        tag.Name,
        tag.Type);
}
