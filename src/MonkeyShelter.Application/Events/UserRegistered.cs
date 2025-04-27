namespace MonkeyShelter.Application.Events;

public record UserRegistered(
        Guid UserId,
        string Email,
        Guid ShelterId,
        DateTime RegisteredAt
        );
