namespace MonkeyShelter.Application;

public record CreateShelterRequest(string Name, string Location);
public record UpdateShelterRequest(string Name, string Location);
public record CreateSpeciesRequest(string Name, string Description);
public record UpdateSpeciesRequest(string Name, string Description);

public record ArriveMonkeyRequest(
    Guid SpeciesId,
    Guid ShelterId,
    string Name,
    float Weight,
    DateTime ArrivalDate);

public record DepartMonkeyRequest(
    Guid MonkeyId,
    DateTime DepartureDate,
    float WeightAtDeparture);

public record UpdateWeightRequest(
    Guid MonkeyId,
    float NewWeight);

public record CompleteVetCheckRequest(
    Guid MonkeyId,
    DateTime CompletedDate);