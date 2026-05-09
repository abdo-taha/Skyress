using Microsoft.Extensions.Logging;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.TagAssignments.Commands.DeleteTagAssignment;

public record DeleteTagAssignmentCommand(long Id) : ICommand;

public class DeleteTagAssignmentCommandHandler : ICommandHandler<DeleteTagAssignmentCommand>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly ILogger<DeleteTagAssignmentCommandHandler> _logger;

    public DeleteTagAssignmentCommandHandler(ITagAssignmentRepository tagAssignmentRepository, ILogger<DeleteTagAssignmentCommandHandler> logger)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteTagAssignmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Command} for Id: {Id}", nameof(DeleteTagAssignmentCommand), request.Id);

        var tagAssignment = await _tagAssignmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (tagAssignment is null)
        {
            return Result.Failure(new Error("", ""));
        }

        await _tagAssignmentRepository.DeleteByIdAsync(request.Id, cancellationToken);
        await _tagAssignmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Command} completed. Id: {Id}", nameof(DeleteTagAssignmentCommand), request.Id);
        return Result.Success();
    }
}