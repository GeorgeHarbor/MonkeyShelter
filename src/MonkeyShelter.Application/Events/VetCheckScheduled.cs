namespace MonkeyShelter.Application.Events;

public record VetCheckScheduled(
        Guid MonkeyId,
        DateTime ScheduledDate
        );