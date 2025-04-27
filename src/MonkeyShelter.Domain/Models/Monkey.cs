namespace MonkeyShelter.Domain;

public class Monkey
{
    public Guid Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required Species Species { get; set; }
    public required Shelter Shelter { get; set; }
    public float CurrentWeight { get; set; }
    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public bool IsActive { get; set; } = true;
}