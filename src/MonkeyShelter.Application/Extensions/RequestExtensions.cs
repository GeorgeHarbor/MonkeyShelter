using MonkeyShelter.Domain;

namespace MonkeyShelter.Application;

public static class RequestExtensions
{
    public static Shelter MapToShelter(this UpdateShelterRequest req, Shelter? shelter = null)
    {
        return new()
        {
            Id = shelter is null ? Guid.NewGuid() : shelter.Id,
            Name = req.Name,
            Location = req.Location
        };
    }
    public static Shelter MapToShelter(this CreateShelterRequest req)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Location = req.Location
        };
    }

    public static Species MapToSpecie(this UpdateSpeciesRequest req, Shelter? shelter = null)
    {
        return new()
        {
            Id = shelter is null ? Guid.NewGuid() : shelter.Id,
            Name = req.Name,
            Description = req.Description
        };
    }
    public static Species MapToSpecie(this CreateSpeciesRequest req)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Description = req.Description
        };
    }
}