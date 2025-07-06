using Skyress.Domain.Enums;

namespace Skyress.API.DTOs.Todos;

public record UpdateTodoStateRequest(TodoState State);