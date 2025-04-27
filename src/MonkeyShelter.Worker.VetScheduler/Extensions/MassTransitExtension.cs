using MassTransit;

using MonkeyShelter.Infrastructure;

namespace MonkeyShelter.Worker.VetScheduler;

public static class MassTransitExtensions
{
    /// <summary>
    /// Configures MassTransit with RabbitMQ, registers all consumers, and starts the bus.
    /// </summary>
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration config)
    {
        services.AddMassTransit(x =>
        {
            // Automatically register all consumers in the Messaging assembly
            x.AddConsumers(typeof(UserRegisteredConsumer).Assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                // Configure RabbitMQ host from configuration
                cfg.Host(config.GetConnectionString("RabbitMq"), h =>
                {
                    h.Username(config["RabbitMq:Username"]!);
                    h.Password(config["RabbitMq:Password"]!);
                });

                // Automatically configure endpoints for all registered consumers
                cfg.ConfigureEndpoints(context);

            });
        });
        return services;
    }
}