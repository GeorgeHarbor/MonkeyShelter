using MassTransit;

using MonkeyShelter.Infrastructure;

namespace MonkeyShelter.Api.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration config)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumers(typeof(UserRegisteredConsumer).Assembly);

            x.UsingRabbitMq((context, cfg) =>
            {

                var rmq = config.GetSection("RabbitMq");
                cfg.Host(rmq["Host"], h =>
                {
                    h.Username(rmq["Username"]!);
                    h.Password(rmq["Password"]!);
                });

                cfg.ConfigureEndpoints(context);

            });
        });
        return services;
    }
}