
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Application;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Infrastructure;

public class ArrivalsRepository(DataContext ctx) : Repository<Arrival>(ctx), IArrivalRepository
{
    public async Task<List<Arrival>> ListWithIncludesAsync(Expression<Func<Arrival, bool>> predicate)
    {
        return await _ctx.Set<Arrival>()
                         .Where(predicate)
                         .Include(m => m.Monkey)
                         .ThenInclude(m => m.Species)
                         .ToListAsync();
    }
}