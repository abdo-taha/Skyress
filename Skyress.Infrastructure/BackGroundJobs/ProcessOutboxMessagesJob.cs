using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using Skyress.Domain.primitives;
using Skyress.Infrastructure.outbox;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.BackGroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob : IJob
{
    private readonly SkyressDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;
    
    public ProcessOutboxMessagesJob(SkyressDbContext dbContext, IPublisher publisher, ILogger<ProcessOutboxMessagesJob> logger)
    {
        _dbContext = dbContext;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        List<OutboxMessage> messages = await _dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .Take(20)
            .ToListAsync(context.CancellationToken);
        
        foreach (var message in messages)
        {
            await ExecuteAndHandleExceptions( async () => await HandleMessage(message, context.CancellationToken));
        }
    }

    private async Task HandleMessage(OutboxMessage message, CancellationToken cancellationToken)
    {
        IDomainEvent? domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(message.Content,
            new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
        if(domainEvent is null) return;
            
        await _publisher.Publish(domainEvent,cancellationToken);
        message.ProcessedOnUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken); 
    }

    private async Task ExecuteAndHandleExceptions(Func<Task> func)
    {
        try
        {
            await func();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}