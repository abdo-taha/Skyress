using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Tags.Commands.UpdateTagName;

public record UpdateTagNameCommand(long Id, string Name) : ICommand;

public class UpdateTagNameCommandHandler : ICommandHandler<UpdateTagNameCommand>
{
    private readonly ITagRepository _tagRepository;

    public UpdateTagNameCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result> Handle(UpdateTagNameCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(request.Id);

        if (tag is null)
        {
            return Result.Failure(new Error("", ""));
        }

        tag.Name = request.Name;
        await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}