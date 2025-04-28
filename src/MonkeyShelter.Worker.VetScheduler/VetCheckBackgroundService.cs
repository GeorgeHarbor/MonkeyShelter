using MassTransit;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Events;
using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Worker.VetScheduler;

public class VetCheckBackgroundService(
        ILogger<VetCheckBackgroundService> log,
        IServiceScopeFactory scopeFactory
            ) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<VetCheckBackgroundService> _log = log;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

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
                var vetRepo = scope.ServiceProvider.GetRequiredService<IVetChecksRepository>();

                var now = DateTime.UtcNow;
                var due = await vetRepo.ListWithIncludesAsync(v => v.ScheduledDate <= now.Date && v.CompletedDate == null);

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
                            next.Id,
                            next.Monkey.Id,
                            next.ScheduledDate
                        ),
                        ctx => ctx.Headers.Set("MT-Message-Name", nameof(VetCheckScheduled)), stoppingToken
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