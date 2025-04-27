// Consumers/AuditEventConsumer.cs
using MassTransit;

namespace MonkeyShelter.Audit.Consumers;

public class AuditEventConsumer(AuditDbContext db) : IConsumer<ConsumeContext>
{
    readonly AuditDbContext _db = db;

    public async Task Consume(ConsumeContext<ConsumeContext> context)
    {
        var eventType = context.ReceiveContext.TransportHeaders.Get<string>("MT-Message-Name")
                        ?? context.Message.GetType().Name;

        var raw = context.ReceiveContext.GetBody().ToArray();
        var payload = System.Text.Encoding.UTF8.GetString(raw);

        var log = new AuditLog
        {
            EventType = eventType,
            Payload = payload
        };
        _db.AuditLogs.Add(log);

        await _db.SaveChangesAsync(context.CancellationToken);
    }
}