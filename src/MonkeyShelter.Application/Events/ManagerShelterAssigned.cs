namespace MonkeyShelter.Application.Events;

public record ManagerShelterAssigned(
        Guid UserId,
        Guid Shelterid,
        DateTime AssignedAt
        );