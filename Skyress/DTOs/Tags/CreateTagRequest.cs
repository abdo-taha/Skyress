using Skyress.Domain.Enums;

namespace Skyress.API.DTOs.Tags;

public record CreateTagRequest(string Name, TagType Type);