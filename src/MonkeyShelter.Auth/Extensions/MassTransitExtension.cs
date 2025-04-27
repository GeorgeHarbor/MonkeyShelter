using MassTransit;

namespace MonkeyShelter.Auth;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration config)
    {
        services.AddMassTransit(x =>
            {
                // Automatically register all consumers in this assembly
                x.AddConsumers(typeof(MassTransitExtensions).Assembly);

                x.UsingRabbitMq((context, cfg) =>
                    // Configure RabbitMQ host from configuration
                    cfg.Host(config.GetConnectionString("RabbitMq"), h =>
                    {
                        h.Username(config["RabbitMq:Username"]!);
                        h.Password(config["RabbitMq:Password"]!);
                    }));
            });


        return services;
    }
}