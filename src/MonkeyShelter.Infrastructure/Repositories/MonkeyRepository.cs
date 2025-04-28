using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Infrastructure.Repositories;

public class MonkeyRepository(DataContext ctx) : Repository<Monkey>(ctx), IMonkeyRepository
{
    public async Task<List<Monkey>> ListAllWithIncludesAsync()
    {
        return await _ctx.Set<Monkey>()
                         .Include(m => m.Species)
                         .Include(m => m.Shelter)
                         .ToListAsync();
    }
}