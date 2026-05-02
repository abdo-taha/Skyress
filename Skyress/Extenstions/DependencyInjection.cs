using Skyress.Infrastructure.Saga;

namespace Skyress.API.Extenstions;

using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Skyress.Infrastructure.Extensions;
using Skyress.Application.Extensions;
using Quartz;
using Skyress.API.OpenApi;
using Skyress.Infrastructure.BackGroundJobs;
using System.Text;

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

        var secretKey = configuration["Jwt:SecretKey"] ?? "default-jwt-secret-key-change-in-production";
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "Skyress",
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"] ?? "SkyressAPI",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        services.AddHttpContextAccessor();

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
        
        services.AddSagas(configuration);
        
        return services;
    }

}
