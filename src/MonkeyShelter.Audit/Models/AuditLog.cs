namespace MonkeyShelter.Audit;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}