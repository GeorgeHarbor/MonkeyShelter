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
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("VetCheckBackgroundService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var vetRepo = scope.ServiceProvider.GetRequiredService<IVetChecksRepository>();

                var now = DateTime.UtcNow;
                var due = await vetRepo.ListWithIncludesAsync(
                    v => v.ScheduledDate <= now.Date && v.CompletedDate == null
                );

                _log.LogInformation("Found {Count} due vet checks", due.Count);

                // Project minimal needed data to avoid entity tracking issues
                var dueList = due.Select(v => new
                {
                    VetCheckId = v.Id,
                    MonkeyId = v.Monkey.Id
                }).ToList();

                var tasks = dueList.Select(async item =>
                {
                    using var innerScope = _scopeFactory.CreateScope();

                    var innerUow = innerScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var innerPublisher = innerScope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                    var innerRepo = innerScope.ServiceProvider.GetRequiredService<IVetChecksRepository>();

                    var vetCheck = await innerRepo.GetByIdWithIncludesAsync(item.VetCheckId, stoppingToken);
                    if (vetCheck == null)
                    {
                        _log.LogWarning("Vet check {Id} not found in parallel scope", item.VetCheckId);
                        return;
                    }

                    vetCheck.CompletedDate = DateTime.UtcNow;
                    await innerUow.SaveChangesAsync(stoppingToken);

                    var next = new VetCheckSchedule
                    {
                        Id = Guid.NewGuid(),
                        Monkey = vetCheck.Monkey,
                        ScheduledDate = DateTime.UtcNow.AddDays(60)
                    };

                    await innerUow.VetChecks.AddAsync(next, stoppingToken);
                    await innerUow.SaveChangesAsync(stoppingToken);

                    await innerPublisher.Publish(
                        new VetCheckScheduled(
                            next.Id,
                            vetCheck.Monkey.Id,
                            next.ScheduledDate
                        ),
                        ctx => ctx.Headers.Set("MT-Message-Name", nameof(VetCheckScheduled)),
                        stoppingToken
                    );

                    _log.LogInformation("Published VetCheckScheduled for Monkey {Id}", vetCheck.Monkey.Id);
                });

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error running vet check scan");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}