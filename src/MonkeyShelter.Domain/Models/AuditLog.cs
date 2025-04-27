using System.Text.Json;

namespace MonkeyShelter.Domain;

public class AuditLog
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTime.Now;
    public JsonDocument? Details { get; set; }
}