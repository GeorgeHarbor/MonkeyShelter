using Microsoft.Extensions.Caching.Distributed;

using MonkeyShelter.Application.Interfaces;

using StackExchange.Redis;

namespace MonkeyShelter.Reports.Services;

public class ReportCacheInvalidator(IDistributedCache cache, IConnectionMultiplexer redis) : IReportCacheInvalidator
{
    private readonly IDistributedCache _cache = cache;
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task InvalidateCountPerSpeciesAsync()
    {
        await _cache.RemoveAsync("report:count-per-species");
    }

    public async Task InvalidateArrivalsInRangeAsync(DateTime arrivalDate)
    {
        var arrivalInt = int.Parse(arrivalDate.ToString("yyyyMMdd"));

        var db = _redis.GetDatabase();
        var server = _redis.GetServer(_redis.GetEndPoints().First());

        var keys = server.KeysAsync(db.Database, pattern: "ReportService:report:arrivals:*:*");

        await foreach (var key in keys)
        {
            var parts = key.ToString().Split(':');
            if (parts.Length < 5) continue;

            if (int.TryParse(parts[^2], out var from) && int.TryParse(parts[^1], out var to))
            {
                if (arrivalInt >= from && arrivalInt <= to)
                    await db.KeyDeleteAsync(key);
            }
        }
    }

    public async Task InvalidateAllAsync(DateTime arrivalDate)
    {
        await InvalidateCountPerSpeciesAsync();
        await InvalidateArrivalsInRangeAsync(arrivalDate);
    }
}