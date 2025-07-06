using Microsoft.EntityFrameworkCore;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Enums;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository;

public class TodoRepository : GenericRepository<Todo>, ITodoRepository
{
    public TodoRepository(SkyressDbContext skyressDbContext) : base(skyressDbContext)
    {
    }

    public async Task<List<Todo>> GetTodosByStateAsync(TodoState state, CancellationToken cancellationToken)
    {
        return await GetAsync(predicate: t => t.State == state).ToListAsync(cancellationToken);
    }
}
