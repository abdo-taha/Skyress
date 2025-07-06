using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Tags.Commands.UpdateTagType;

public record UpdateTagTypeCommand(long Id, TagType Type) : ICommand;

public class UpdateTagTypeCommandHandler : ICommandHandler<UpdateTagTypeCommand>
{
    private readonly ITagRepository _tagRepository;

    public UpdateTagTypeCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result> Handle(UpdateTagTypeCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(request.Id);

        if (tag is null)
        {
            return Result.Failure(new Error("Tag not found", ""));
        }

        tag.Type = request.Type;
        await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}