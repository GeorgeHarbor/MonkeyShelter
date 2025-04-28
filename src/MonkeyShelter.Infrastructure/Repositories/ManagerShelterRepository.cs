
using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Infrastructure.Repositories;

public class ManagerShelterRepository(DataContext ctx) : Repository<ManagerShelter>(ctx), IManagerShelterRepository
{
    public async Task<bool> IsManagerOfShelterAsync(Guid userId, Guid shelterId)
    {
        var result = await _ctx.ManagerShelters.FirstOrDefaultAsync(ms => ms.ManagerId == userId && ms.ShelterId == shelterId);
        return result is not null;
    }
}