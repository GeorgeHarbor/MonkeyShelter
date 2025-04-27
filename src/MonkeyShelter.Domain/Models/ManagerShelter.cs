namespace MonkeyShelter.Domain;

public class ManagerShelter
{
    public Guid ManagerId { get; set; }
    public Guid ShelterId { get; set; }
    public Shelter Shelter { get; set; } = null!;
}