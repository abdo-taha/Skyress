namespace Skyress.Application.TagAssignments.Responses;

using Skyress.Domain.Aggregates.TagAssignment;

public sealed record TagAssignmentResponse(
    long Id,
    long TagId,
    long ItemId)
{
    public static TagAssignmentResponse FromDomain(TagAssignment assignment) => new(
        assignment.Id,
        assignment.TagId,
        assignment.ItemId);
}
