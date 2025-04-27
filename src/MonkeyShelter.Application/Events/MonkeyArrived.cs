namespace MonkeyShelter.Application.Events;

public record MonkeyArrived(
       Guid MonkeyId,
       Guid SpeciesId,
       Guid ShelterId,
       DateTime ArrivalDate,
       float WeightAtArrival
        );