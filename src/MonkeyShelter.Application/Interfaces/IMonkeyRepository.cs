using System.Linq.Expressions;

using MonkeyShelter.Domain;

namespace MonkeyShelter.Application.Interfaces;

public interface IMonkeyRepository : IRepository<Monkey>
{
    Task<List<Monkey>> ListAllWithIncludesAsync();

    Task<List<Monkey>> ListWithIncludesAsync(Expression<Func<Monkey, bool>> predicate);

    Task<Monkey?> GetByIdWithIncludesAsync(Expression<Func<Monkey, bool>> predicate);
}