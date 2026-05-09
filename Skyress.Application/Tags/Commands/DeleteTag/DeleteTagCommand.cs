using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Tags.Commands.DeleteTag;

public record DeleteTagCommand(long Id) : ICommand;

public class DeleteTagCommandHandler : ICommandHandler<DeleteTagCommand>
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<DeleteTagCommandHandler> _logger;

    public DeleteTagCommandHandler(ITagRepository tagRepository, ILogger<DeleteTagCommandHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command} for TagId: {Id}", nameof(DeleteTagCommand), request.Id);

        await _tagRepository.DeleteByIdAsync(request.Id, cancellationToken);

        await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. TagId: {Id}", nameof(DeleteTagCommand), request.Id);
        return Result.Success();
    }
}