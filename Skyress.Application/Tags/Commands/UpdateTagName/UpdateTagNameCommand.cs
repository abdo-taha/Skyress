using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Tags.Commands.UpdateTagName;

public record UpdateTagNameCommand(long Id, string Name) : ICommand;

public class UpdateTagNameCommandHandler : ICommandHandler<UpdateTagNameCommand>
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<UpdateTagNameCommandHandler> _logger;

    public UpdateTagNameCommandHandler(ITagRepository tagRepository, ILogger<UpdateTagNameCommandHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateTagNameCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command}", nameof(UpdateTagNameCommand));

        var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);

        if (tag is null)
        {
            return Result.Failure(new Error("", ""));
        }

        tag.Name = request.Name;
        await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed", nameof(UpdateTagNameCommand));
        return Result.Success();
    }
}