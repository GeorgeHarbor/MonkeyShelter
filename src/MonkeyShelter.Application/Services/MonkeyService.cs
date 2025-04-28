using MonkeyShelter.Application.Exceptions;
using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Application;

public class MonkeyService(IUnitOfWork uow, IMonkeyRepository repo) : IMonkeyService
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IMonkeyRepository _repo = repo;

    public async Task<Monkey> ArriveMonkeyAsync(ArriveMonkeyRequest req, CancellationToken ct = default)
    {
        var today = req.ArrivalDate.Date;
        var arrivalsCount = await _uow.Arrivals
            .CountAsync(a => a.Date.Date == today, ct);

        if (arrivalsCount >= 7)
            throw new BusinessRuleException(
                $"Cannot arrive more than 7 monkeys on {today:yyyy-MM-dd}.");

        var species = await _uow.Species.GetByIdAsync(req.SpeciesId, ct)
            ?? throw new EntityNotFoundException(nameof(Species), req.SpeciesId);

        var shelter = await _uow.Shelters.GetByIdAsync(req.ShelterId, ct)
            ?? throw new EntityNotFoundException(nameof(Shelter), req.ShelterId);

        var monkeysInShelter = await _repo.ListWithIncludesAsync(m => m.Species.Id == req.SpeciesId);

        var monkeyCount = monkeysInShelter.Count;

        if (monkeyCount > 90)
            throw new BusinessRuleException("Shelters can only house up to 90 monkeys.");

        var speciesCount = monkeysInShelter.Select(m => m.Species).Count();

        if (speciesCount > 15)
            throw new BusinessRuleException("Shelters can only house up to 15 species.");

        var monkey = new Monkey()
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Species = species,
            Shelter = shelter,
            CurrentWeight = req.Weight,
            ArrivalDate = DateTime.UtcNow
        };
        await _uow.Monkeys.AddAsync(monkey, ct);

        var arrival = new Arrival()
        {
            Id = Guid.NewGuid(),
            Monkey = monkey,
            Date = req.ArrivalDate,
            WeightAtArrival = req.Weight
        };
        await _uow.Arrivals.AddAsync(arrival, ct);

        await _uow.SaveChangesAsync(ct);

        return monkey;
    }

    public async Task<Monkey> DepartMonkeyAsync(DepartMonkeyRequest req, CancellationToken ct = default)
    {
        var today = req.DepartureDate.Date;
        var todaysDepartures = await _uow.Departures.CountAsync(d => d.Date.Date == today, ct);
        if (todaysDepartures > 5)
            throw new BusinessRuleException($"Cannot have more then 5 departures in a single day.");

        var todaysArrivals = await _uow.Arrivals.CountAsync(a => a.Date.Date == today, ct);
        if ((todaysDepartures - todaysArrivals) > 2)
            throw new BusinessRuleException($"Departures minus arrivals cannot exceed 2.");

        var monkey = await _repo.GetByIdWithIncludesAsync(m => m.Id == req.MonkeyId) ?? throw new EntityNotFoundException(nameof(Monkey), req.MonkeyId);

        var monkeysInSpecies = await _repo.ListWithIncludesAsync(m => m.Species.Id == monkey.Species.Id);

        if (monkeysInSpecies.Count <= 1)
            throw new BusinessRuleException($"At least one monkey of a species must be in shelter.");

        Departure departure = new()
        {
            Id = Guid.NewGuid(),
            Monkey = monkey,
            Date = today,
            WeightAtDeparture = req.WeightAtDeparture
        };

        await _uow.Departures.AddAsync(departure, ct);

        monkey.IsActive = false;
        monkey.DepartureDate = today;
        monkey.CurrentWeight = req.WeightAtDeparture;

        _uow.Monkeys.Update(monkey);

        await _uow.SaveChangesAsync(ct);
        return monkey;
    }

    public async Task<(float, float)> UpdateWeightAsync(UpdateWeightRequest req, CancellationToken ct = default)
    {
        var monkey = await _uow.Monkeys.GetByIdAsync(req.MonkeyId, ct) ?? throw new EntityNotFoundException(nameof(Monkey), req.MonkeyId);
        var oldWeight = monkey.CurrentWeight;

        monkey.CurrentWeight = req.NewWeight;

        await _uow.SaveChangesAsync(ct);
        return (oldWeight, monkey.CurrentWeight);
    }
}