using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Skyress.Application.Behaviors;

namespace Skyress.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(assembly)
        );
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
