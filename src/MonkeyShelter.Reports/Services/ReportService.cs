using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Interfaces;

namespace MonkeyShelter.Reports;

public class ReportService(IDistributedCache cache, IMonkeyRepository monkeyRepo, IArrivalRepository arrivalRepo)
{
    private readonly IDistributedCache _cache = cache;
    private readonly IMonkeyRepository _monkeyRepo = monkeyRepo;
    private readonly IArrivalRepository _arrivalRepo = arrivalRepo;

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

        return data;
    }
    public async Task<Dictionary<string, int>> GetArrivalsInRangeAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        // 1) Build a cache key that encodes the date range
        string cacheKey = $"report:arrivals:{from:yyyyMMdd}:{to:yyyyMMdd}";

        // 2) Try to get from Redis
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<Dictionary<string, int>>(cached)
                   ?? [];
        }
        var fromUtc = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(to.Date, DateTimeKind.Utc);
        // 3) If missing, query your DB for arrivals in [from…to]
        var arrivals = await _arrivalRepo.ListWithIncludesAsync(a => a.Date <= toUtc && a.Date >= fromUtc);

        // 4) Aggregate counts by species
        var result = arrivals
            .Select(m => m.Monkey)
            .GroupBy(a => a.Species.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        // 5) Cache the JSON for 5 minutes (tune as you like)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        var payload = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(cacheKey, payload, cacheOptions, cancellationToken);

        return result;
    }
}