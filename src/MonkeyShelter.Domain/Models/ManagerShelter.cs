namespace MonkeyShelter.Domain;

public class ManagerShelter
{
    public Guid ManagerId { get; set; }
    public required Shelter Shelter { get; set; }
}