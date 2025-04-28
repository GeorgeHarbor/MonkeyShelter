using MonkeyShelter.Domain;

namespace MonkeyShelter.Application.Interfaces;

public interface IMonkeyRepository : IRepository<Monkey>
{
    Task<List<Monkey>> ListAllWithIncludesAsync();
}