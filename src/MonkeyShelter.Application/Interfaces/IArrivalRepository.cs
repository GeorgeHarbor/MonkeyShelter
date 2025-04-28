using System.Linq.Expressions;

using MonkeyShelter.Domain;

namespace MonkeyShelter.Application;

public interface IArrivalRepository : IRepository<Arrival>
{
    Task<List<Arrival>> ListWithIncludesAsync(Expression<Func<Arrival, bool>> predicate);
}