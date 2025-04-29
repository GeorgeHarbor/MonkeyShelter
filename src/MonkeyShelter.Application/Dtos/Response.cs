namespace MonkeyShelter.Application.Dtos;

public record MonkeyResponse(string Id, string Name, float Weight, string Species, string ShelterName, string ArrivalDate, bool IsActive, float CurrentWeight);