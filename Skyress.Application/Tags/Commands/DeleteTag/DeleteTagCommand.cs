using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Tags.Commands.DeleteTag;

public record DeleteTagCommand(long Id) : ICommand;

public class DeleteTagCommandHandler : ICommandHandler<DeleteTagCommand>
{
    private readonly ITagRepository _tagRepository;

    public DeleteTagCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        await _tagRepository.DeleteByIdAsync(request.Id);

        await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}