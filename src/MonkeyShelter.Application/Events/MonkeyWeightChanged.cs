namespace MonkeyShelter.Application.Events;

public record MonkeyWeightChanged(
        Guid MonkeyId,
        float OldWeight,
        float NewWeight,
        DateTime ChangeDate
        );