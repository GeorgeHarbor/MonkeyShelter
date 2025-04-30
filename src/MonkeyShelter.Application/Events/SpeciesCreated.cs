namespace MonkeyShelter.Application.Events;

public record SpeciesCreated(
        Guid SpeciesId,
        string Name,
        string Description,
        DateTime CreatedAt
        );

public record SpeciesUpdated(
        Guid SpeciesId,
        string Name,
        string Description,
        DateTime CreatedAt
        );