namespace MonkeyShelter.Application.Events;

public record ShelterCreated(
        Guid ShelterId,
        string Name,
        string Location,
        DateTime CreatedAt
        );

public record ShelterUpdated(
        Guid ShelterId,
        string Name,
        string Location,
        DateTime CreatedAt
        );