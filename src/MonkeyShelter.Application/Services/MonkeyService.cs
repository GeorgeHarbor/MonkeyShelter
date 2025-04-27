using MonkeyShelter.Application.Exceptions;
using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Application;

public class MonkeyService(IUnitOfWork uow) : IMonkeyService
{
    private readonly IUnitOfWork _uow = uow;

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

    public async Task CompleteVetCheckAsync(
        CompleteVetCheckRequest req,
        CancellationToken ct = default)
    {
        var schedule = await _uow.VetChecks
            .ListAsync(v =>
                v.Monkey.Id == req.MonkeyId &&
                v.ScheduledDate.Date == DateTime.Now.Date &&
                v.CompletedDate == null,
                ct);

        var entry = schedule[0]
            ?? throw new BusinessRuleException(
                $"No pending vet check scheduled on {DateTime.Now.Date:yyyy-MM-dd} for monkey {req.MonkeyId}.");

        entry.CompletedDate = req.CompletedDate;

        var next = new VetCheckSchedule
        {
            Id = Guid.NewGuid(),
            Monkey = entry.Monkey,
            ScheduledDate = req.CompletedDate.AddDays(60)
        };
        await _uow.VetChecks.AddAsync(next, ct);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DepartMonkeyAsync(DepartMonkeyRequest req, CancellationToken ct = default)
    {
        var today = req.DepartureDate.Date;
        var todaysDepartures = await _uow.Departures.CountAsync(d => d.Date.Date == today, ct);
        if (todaysDepartures > 5)
            throw new BusinessRuleException($"Cannot have more then 5 departures in a single day.");

        var todaysArrivals = await _uow.Arrivals.CountAsync(a => a.Date.Date == today, ct);
        if ((todaysDepartures - todaysArrivals) > 2)
            throw new BusinessRuleException($"Departures minus arrivals cannot exceed 2.");

        var monkey = await _uow.Monkeys.GetByIdAsync(req.MonkeyId, ct) ?? throw new EntityNotFoundException(nameof(Monkey), req.MonkeyId);

        var monkeysInSpeciesCount = await _uow.Monkeys.CountAsync(m => m.Species.Id == monkey.Species.Id && m.IsActive, ct);

        if (monkeysInSpeciesCount <= 1)
            throw new BusinessRuleException($"At least one monkey of a species must be in shelter.");

        Departure departure = new()
        {
            Id = Guid.NewGuid(),
            Monkey = monkey,
            Date = today,
            WeightAtDeparture = monkey.CurrentWeight
        };

        await _uow.Departures.AddAsync(departure, ct);

        monkey.IsActive = false;
        monkey.DepartureDate = today;

        _uow.Monkeys.Update(monkey);

        await _uow.SaveChangesAsync(ct);
    }

    public async Task UpdateWeightAsync(UpdateWeightRequest req, CancellationToken ct = default)
    {
        var monkey = await _uow.Monkeys.GetByIdAsync(req.MonkeyId, ct) ?? throw new EntityNotFoundException(nameof(Monkey), req.MonkeyId);

        monkey.CurrentWeight = req.NewWeight;

        await _uow.SaveChangesAsync(ct);
    }
}