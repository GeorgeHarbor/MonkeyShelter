
using MassTransit;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Events;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Worker.VetScheduler;

public class VetCheckBackgroundService(
        ILogger<VetCheckBackgroundService> log,
        IServiceScopeFactory scopeFactory
            ) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<VetCheckBackgroundService> _log = log;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("VetCheckBackgroundService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var now = DateTime.UtcNow;
                var due = await uow.VetChecks.ListAsync(v => v.ScheduledDate <= now && v.CompletedDate == null, stoppingToken);

                foreach (var v in due)
                {
                    v.CompletedDate = now;
                    await uow.SaveChangesAsync(stoppingToken);


                    var next = new VetCheckSchedule
                    {
                        Id = Guid.NewGuid(),
                        Monkey = v.Monkey,
                        ScheduledDate = now.AddDays(60)
                    };
                    await uow.VetChecks.AddAsync(next, stoppingToken);
                    await uow.SaveChangesAsync(stoppingToken);
                    await publisher.Publish(
                        new VetCheckScheduled(
                            next.Id,            // ← your event’s own Id
                            next.Monkey.Id,     // ← the monkey’s Id
                            next.ScheduledDate  // ← the scheduled date
                        ),
                        stoppingToken
                    );
                    _log.LogInformation("Event published");
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error running bet check scan");
                throw;
            }
            await Task.Delay(_interval, stoppingToken);
        }
    }
}