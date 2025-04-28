using System.Text.Json;

using MassTransit;

using Microsoft.Extensions.Caching.Distributed;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Events;
using MonkeyShelter.Application.Interfaces;

namespace MonkeyShelter.Reports;

public class ReportService(IDistributedCache cache, IMonkeyRepository monkeyRepo, IArrivalRepository arrivalRepo, IPublishEndpoint publisher)
{
    private readonly IDistributedCache _cache = cache;
    private readonly IMonkeyRepository _monkeyRepo = monkeyRepo;
    private readonly IArrivalRepository _arrivalRepo = arrivalRepo;
    private readonly IPublishEndpoint _publisher = publisher;

    public async Task<Dictionary<string, int>> GetCountPerSpeciesAsync()
    {
        const string cacheKey = "report:count-per-species";
        var json = await _cache.GetStringAsync(cacheKey);

        if (json is not null)
            return JsonSerializer.Deserialize<Dictionary<string, int>>(json)!;

        var monkeys = await _monkeyRepo.ListAllWithIncludesAsync();

        var data = monkeys
                    .GroupBy(m => m.Species.Name)
                    .ToDictionary(g => g.Key, g => g.Count());


        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(data), options);

        await _publisher.Publish(new ReportGenerated(cacheKey, JsonSerializer.Serialize(data), DateTime.UtcNow),
                ctx => ctx.Headers.Set("MT-Message-Name", nameof(ReportGenerated)));
        return data;
    }
    public async Task<Dictionary<string, int>> GetArrivalsInRangeAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = $"report:arrivals:{from:yyyyMMdd}:{to:yyyyMMdd}";

        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<Dictionary<string, int>>(cached)
                   ?? [];
        }
        var fromUtc = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(to.Date, DateTimeKind.Utc);

        var arrivals = await _arrivalRepo.ListWithIncludesAsync(a => a.Date <= toUtc && a.Date >= fromUtc);

        var result = arrivals
            .Select(m => m.Monkey)
            .GroupBy(a => a.Species.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
        };
        var payload = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(cacheKey, payload, cacheOptions, cancellationToken);

        await _publisher.Publish(new ReportGenerated(cacheKey, JsonSerializer.Serialize(payload), DateTime.UtcNow),
                ctx => ctx.Headers.Set("MT-Message-Name", nameof(ReportGenerated)), cancellationToken);
        return result;
    }
}