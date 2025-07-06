using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Enums;

namespace Skyress.Application.Contracts.Persistence;

public interface ITodoRepository : IGenericRepository<Todo>
{
    Task<List<Todo>> GetTodosByStateAsync(TodoState state, CancellationToken cancellationToken);
}
