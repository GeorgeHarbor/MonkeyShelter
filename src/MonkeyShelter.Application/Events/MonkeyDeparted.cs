
namespace MonkeyShelter.Application.Events;

public record MonkeyDeparted(
       Guid MonkeyId,
       Guid SpeciesId,
       Guid ShelterId,
       DateTime DepartureDate,
       float WeightAtDeparture
        );