using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Common;
using Skyress.Domain.Enums;

namespace Skyress.Application.Tags.Commands.CreateTag;

public record CreateTagCommand(string Name, TagType Type) : ICommand<Tag>;

public class CreateTagCommandHandler : ICommandHandler<CreateTagCommand, Tag>
{
    private readonly ITagRepository _tagRepository;

    public CreateTagCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<Tag>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = new Tag(request.Name, request.Type);

        await _tagRepository.CreateAsync(tag);
        
        await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(tag);
    }
}