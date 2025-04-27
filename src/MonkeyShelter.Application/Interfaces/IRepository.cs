using System.Linq.Expressions;

namespace MonkeyShelter.Application;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default);

    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);

    Task<int> CountAsync(CancellationToken ct = default);
    Task<int> CountAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default);
}