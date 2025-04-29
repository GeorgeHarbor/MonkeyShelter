using MassTransit;
using MassTransit.Internals;

using Microsoft.Extensions.Caching.Distributed;

using MonkeyShelter.Application.Events;
using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Infrastructure;
using MonkeyShelter.Reports;

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

                cfg.ReceiveEndpoint("reports-queue", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    async Task HandleAuditEvent<T>(ConsumeContext<T> ctx) where T : class
                    {
                        using var scope = context.GetRequiredService<IServiceScopeFactory>().CreateScope();
                        var cache = scope.ServiceProvider.GetRequiredService<IReportCacheInvalidator>();

                        var date = DateTime.Now;
                        await cache.InvalidateAllAsync(date);
                    }
                    
                    e.Bind<MonkeyArrived>();
                    e.Handler<MonkeyArrived>(ctx => HandleAuditEvent(ctx));
                });
            });
        });
        return services;
    }
}