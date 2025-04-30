using System.Linq.Expressions;

using MonkeyShelter.Domain;

namespace MonkeyShelter.Application.Interfaces;

public interface IVetChecksRepository : IRepository<VetCheckSchedule>
{
    Task<List<VetCheckSchedule>> ListWithIncludesAsync(Expression<Func<VetCheckSchedule, bool>> predicate);

    Task<VetCheckSchedule> GetByIdWithIncludesAsync(Guid id, CancellationToken ct = default);
}