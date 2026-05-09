using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.TagAssignments.Responses;
using Skyress.Domain.Aggregates.TagAssignment;
using Skyress.Domain.Common;

namespace Skyress.Application.TagAssignments.Commands.CreateTagAssignment;

public record CreateTagAssignmentCommand(long TagId, long ItemId) : ICommand<TagAssignmentResponse>;

public class CreateTagAssignmentCommandHandler : ICommandHandler<CreateTagAssignmentCommand, TagAssignmentResponse>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<CreateTagAssignmentCommandHandler> _logger;

    public CreateTagAssignmentCommandHandler(
        ITagAssignmentRepository tagAssignmentRepository,
        ITagRepository tagRepository,
        ILogger<CreateTagAssignmentCommandHandler> logger)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result<TagAssignmentResponse>> Handle(CreateTagAssignmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(CreateTagAssignmentCommand));

        var tag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken);
        if (tag is null)
        {
            return Result<TagAssignmentResponse>.Failure(new Error(" ", ""));
        }

        var tagAssignment = new TagAssignment
        {
            TagId = request.TagId,
            ItemId = request.ItemId
        };

        await _tagAssignmentRepository.CreateAsync(tagAssignment, cancellationToken);
        await _tagAssignmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(CreateTagAssignmentCommand), tagAssignment.Id);
        return Result.Success(TagAssignmentResponse.FromDomain(tagAssignment));
    }
}