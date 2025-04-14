using Skyress.Domain.Aggregates.Todo;

namespace Skyress.Application.Contracts.Persistence;

public interface ITodoRepository : IGenericRepository<Todo>
{
}
