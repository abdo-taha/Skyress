﻿using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

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
        return services;
    }
}
