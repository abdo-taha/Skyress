namespace Skyress.API.Extenstions;

using Skyress.Infrastructure.Extensions;
using Skyress.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();
        services
            .AddInfrasructure(configuration)
            .AddApplication();

        return services;
    }

}
