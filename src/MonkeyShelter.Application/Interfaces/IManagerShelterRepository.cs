using MonkeyShelter.Domain;

namespace MonkeyShelter.Application.Interfaces;

public interface IManagerShelterRepository : IRepository<ManagerShelter>
{
    Task<bool> IsManagerOfShelterAsync(Guid userId, Guid shelterId);
}