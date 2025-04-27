using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Application;

namespace MonkeyShelter.Infrastructure;

public class Repository<T>(DataContext ctx) : IRepository<T> where T : class
{
    protected readonly DataContext _ctx = ctx;

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _ctx.Set<T>().AddAsync(entity, ct);
    }

    public void Delete(T entity)
    {
        _ctx.Set<T>().Remove(entity);
    }

    public void Update(T entity)
    {
        _ctx.Set<T>().Update(entity);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _ctx.Set<T>().FindAsync([id], ct);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken ct = default)
    {
        return await _ctx.Set<T>().ToListAsync(ct);
    }

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await _ctx.Set<T>()
            .Where(predicate)
            .ToListAsync(ct);
    }

}