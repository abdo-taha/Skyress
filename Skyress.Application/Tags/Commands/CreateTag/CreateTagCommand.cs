using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Application.Tags.Responses;
using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Tags.Commands.CreateTag;

public record CreateTagCommand(string Name, TagType Type) : ICommand<TagResponse>;

public class CreateTagCommandHandler : ICommandHandler<CreateTagCommand, TagResponse>
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<CreateTagCommandHandler> _logger;

    public CreateTagCommandHandler(ITagRepository tagRepository, ILogger<CreateTagCommandHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result<TagResponse>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(CreateTagCommand));

        var tag = new Tag(request.Name, request.Type);

        await _tagRepository.CreateAsync(tag, cancellationToken);

        await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(CreateTagCommand), tag.Id);
        return Result.Success(TagResponse.FromDomain(tag));
    }
}