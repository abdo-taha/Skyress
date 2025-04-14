using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Todo;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository;

public class TodoRepository : GenericRepository<Todo>, ITodoRepository
{
    public TodoRepository(SkyressDbContext skyressDbContext) : base(skyressDbContext)
    {
    }
}
