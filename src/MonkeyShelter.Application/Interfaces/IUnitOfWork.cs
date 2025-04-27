
using MonkeyShelter.Domain;

namespace MonkeyShelter.Application;

public interface IUnitOfWork
{
    IRepository<Monkey> Monkeys { get; }
    IRepository<Arrival> Arrivals { get; }
    IRepository<Departure> Departures { get; }
    IRepository<WeightHistory> WeightHistories { get; }
    IRepository<VetCheckSchedule> VetChecks { get; }
    IRepository<ReportCache> ReportCaches { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<ManagerShelter> ManagerShelters { get; }
    IRepository<Shelter> Shelters { get; }
    IRepository<Species> Species { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}