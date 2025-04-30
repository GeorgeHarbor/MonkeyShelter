
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Infrastructure;

public class VetChecksRepository(DataContext ctx) : Repository<VetCheckSchedule>(ctx), IVetChecksRepository
{
    public async Task<List<VetCheckSchedule>> ListWithIncludesAsync(Expression<Func<VetCheckSchedule, bool>> predicate)
    {
        return await _ctx.Set<VetCheckSchedule>()
                         .Where(predicate)
                         .Include(m => m.Monkey)
                         .ToListAsync();
    }

    public async Task<VetCheckSchedule> GetByIdWithIncludesAsync(Guid id, CancellationToken ct = default)
    {
        return await _ctx.Set<VetCheckSchedule>()
                         .Include(m => m.Monkey)
                         .Where(m => m.Id == id)
                         .FirstAsync(ct);
    }

}