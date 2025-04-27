namespace MonkeyShelter.Domain;

public class Departure
{
    public Guid Id { get; set; }
    public required Monkey Monkey { get; set; }
    public DateTime Date { get; set; }
    public float WeightAtDeparture { get; set; }
}