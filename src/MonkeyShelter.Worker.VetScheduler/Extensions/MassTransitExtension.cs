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
                var rmq = config.GetSection("RabbitMq");
                cfg.Host(rmq["Host"], h =>
                        {
                            h.Username(rmq["Username"]!);
                            h.Password(rmq["Password"]!);
                        });

                // Automatically configure endpoints for all registered consumers
                cfg.ConfigureEndpoints(context);

            });
        });
        return services;
    }
}