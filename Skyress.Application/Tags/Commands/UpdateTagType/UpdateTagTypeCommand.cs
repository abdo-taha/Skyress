using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Tags.Commands.UpdateTagType;

public record UpdateTagTypeCommand(long Id, TagType Type) : ICommand;

public class UpdateTagTypeCommandHandler : ICommandHandler<UpdateTagTypeCommand>
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<UpdateTagTypeCommandHandler> _logger;

    public UpdateTagTypeCommandHandler(ITagRepository tagRepository, ILogger<UpdateTagTypeCommandHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateTagTypeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateTagTypeCommand));

        var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);

        if (tag is null)
        {
            return Result.Failure(new Error("Tag not found", ""));
        }

        tag.Type = request.Type;
        await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateTagTypeCommand));
        return Result.Success();
    }
}