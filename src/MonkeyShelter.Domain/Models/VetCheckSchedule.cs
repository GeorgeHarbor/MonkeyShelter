namespace MonkeyShelter.Domain;

public class VetCheckSchedule
{
    public Guid Id { get; set; }
    public required Monkey Monkey { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}