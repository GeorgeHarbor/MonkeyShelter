
using MassTransit;

using Microsoft.Extensions.Caching.Distributed;

using MonkeyShelter.Application.Events;

using StackExchange.Redis;

namespace MonkeyShelter.Reports;

public class MonkeyArrivedConsumer(IDistributedCache cache, IConnectionMultiplexer redis) : IConsumer<MonkeyArrived>
{
    private readonly IDistributedCache _cache = cache;
    private readonly IConnectionMultiplexer _redis = redis;
    public async Task Consume(ConsumeContext<MonkeyArrived> context)
    {
        await _cache.RemoveAsync("reports:count-by-species");

        var server = _redis.GetServer(_redis.GetEndPoints()[0]);
        await foreach (var key in server.KeysAsync(pattern: "report:arrivals:*"))
        {
            await _cache.RemoveAsync(key!);
        }
    }
}