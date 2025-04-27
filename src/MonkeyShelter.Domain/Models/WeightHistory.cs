namespace MonkeyShelter.Domain;

public class WeightHistory
{
    public Guid Id { get; set; }
    public required Monkey Monkey { get; set; }
    public DateTime ChangeDate { get; set; }
    public float OldWeight { get; set; }
    public float NewWeight { get; set; }
}