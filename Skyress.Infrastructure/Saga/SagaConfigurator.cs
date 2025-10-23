using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skyress.Application.Checkout.Sagas;
using Skyress.Application.Checkout.Sagas.Consumers;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Saga;

public static  class SagaConfigurator
{
    public static IServiceCollection AddSagas(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.AddConsumers(typeof(ReserveItemsRequestedConsumer).Assembly);
            configurator.AddSagaStateMachine<CheckoutStateMachine, CheckoutSagaData>()
                .EntityFrameworkRepository(r =>
                {
                    r.ExistingDbContext<SkyressDbContext>();
                    r.UsePostgres();
                }
        );

            configurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration.GetConnectionString("RabbitMq")!),
                    hst =>
                    {
                        hst.Username(username:"guest");
                        hst.Password(password:"guest");
                    });
                cfg.UseInMemoryOutbox(context);
                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}