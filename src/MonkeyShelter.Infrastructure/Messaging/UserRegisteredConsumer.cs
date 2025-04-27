
using MassTransit;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Events;
using MonkeyShelter.Application.Exceptions;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Infrastructure;

public class UserRegisteredConsumer(IUnitOfWork uow) : IConsumer<UserRegistered>
{
    private readonly IUnitOfWork _uow = uow;

    public async Task Consume(ConsumeContext<UserRegistered> context)
    {
        var msg = context.Message;

        var shelter = await _uow.Shelters.GetByIdAsync(msg.ShelterId, context.CancellationToken);

        if (shelter is null)
            throw new EntityNotFoundException(nameof(Shelter), msg.ShelterId);

        await _uow.ManagerShelters.AddAsync(new ManagerShelter
        {
            ManagerId = msg.UserId,
            Shelter = shelter
        }, context.CancellationToken);

        await _uow.SaveChangesAsync(context.CancellationToken);
    }
}