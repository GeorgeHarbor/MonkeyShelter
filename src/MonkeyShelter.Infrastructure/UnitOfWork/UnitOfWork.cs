﻿using MonkeyShelter.Application;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Infrastructure;

public class UnitOfWork(
    DataContext context,
    IRepository<Monkey> monkeys,
    IRepository<Arrival> arrivals,
    IRepository<Departure> departures,
    IRepository<WeightHistory> weightHistories,
    IRepository<VetCheckSchedule> vetChecks,
    IRepository<ReportCache> reportCaches,
    IRepository<ManagerShelter> managerShelters,
    IRepository<Shelter> shelters,
    IRepository<Species> species) : IUnitOfWork, IDisposable
{
    private readonly DataContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public IRepository<Monkey> Monkeys { get; } = monkeys;
    public IRepository<Arrival> Arrivals { get; } = arrivals;
    public IRepository<Departure> Departures { get; } = departures;
    public IRepository<WeightHistory> WeightHistories { get; } = weightHistories;
    public IRepository<VetCheckSchedule> VetChecks { get; } = vetChecks;
    public IRepository<ReportCache> ReportCaches { get; } = reportCaches;
    public IRepository<ManagerShelter> ManagerShelters { get; } = managerShelters;
    public IRepository<Shelter> Shelters { get; } = shelters;
    public IRepository<Species> Species { get; } = species;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _context.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}