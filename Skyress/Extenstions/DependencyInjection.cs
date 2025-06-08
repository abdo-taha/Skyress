namespace Skyress.API.Extenstions;

using Asp.Versioning;
using Skyress.Infrastructure.Extensions;
using Skyress.Application.Extensions;
using Quartz;
using Skyress.API.OpenApi;
using Skyress.Infrastructure.BackGroundJobs;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSwaggerGen(); 
        services.ConfigureOptions<ConfigureSwaggerGenOptions>();
        services
            .AddInfrasructure(configuration)
            .AddApplication();
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1);
            options.ApiVersionReader = new QueryStringApiVersionReader("api-version");

        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat =  "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        services.AddEndpointsApiExplorer();

        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));
            configure.AddJob<ProcessOutboxMessagesJob>(jobKey)
                .AddTrigger(trigger => trigger.ForJob(jobKey)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(10)
                            .RepeatForever()));
        })
        .AddQuartzHostedService();
        

        return services;
    }

}
