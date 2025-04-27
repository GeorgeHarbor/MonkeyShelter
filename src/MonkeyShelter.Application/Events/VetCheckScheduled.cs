namespace MonkeyShelter.Application.Events;

public record VetCheckScheduled(
        Guid Id,
        Guid MonkeyId,
        DateTime ScheduledDate
        );