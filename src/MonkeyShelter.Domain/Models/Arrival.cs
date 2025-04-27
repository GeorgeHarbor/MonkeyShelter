namespace MonkeyShelter.Domain;

public class Arrival
{
    public Guid Id { get; set; }
    public required Monkey Monkey { get; set; }
    public DateTime Date { get; set; }
    public float WeightAtArrival { get; set; }
}