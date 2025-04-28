using Carter;

using MassTransit;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Events;

namespace MonkeyShelter.Api;

public class ShelterEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/shelters").WithTags("Shelters");

        grp.MapGet("", GetAll);
        grp.MapGet("/{id}", GetById);
        grp.MapPost("", CreateShelter).RequireAuthorization();
        grp.MapDelete("", DeleteShelter).RequireAuthorization();
        grp.MapPut("/{id}", UpdateShelter).RequireAuthorization();
    }

    private async Task<IResult> UpdateShelter(IUnitOfWork uow, Guid id, UpdateShelterRequest req)
    {
        var shelter = await uow.Shelters.GetByIdAsync(id);

        if (shelter is null)
            return TypedResults.NotFound();

        shelter = req.MapToShelter();

        uow.Shelters.Update(shelter);
        await uow.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private async Task<IResult> CreateShelter(IUnitOfWork uow, CreateShelterRequest req, IPublishEndpoint publisher, CancellationToken ct)
    {
        var shelter = req.MapToShelter();
        await uow.Shelters.AddAsync(shelter, ct);
        await uow.SaveChangesAsync(ct);

        await publisher.Publish(new ShelterCreated(shelter.Id, shelter.Name, shelter.Location, DateTime.UtcNow),
                ctx => ctx.Headers.Set("MT-Message-Name", nameof(ShelterCreated)), ct);
        return TypedResults.Ok();
    }

    private async Task<IResult> DeleteShelter(IUnitOfWork uow, Guid id)
    {
        var shelter = await uow.Shelters.GetByIdAsync(id);
        if (shelter is null)
            return TypedResults.BadRequest("");

        uow.Shelters.Delete(shelter);
        await uow.SaveChangesAsync();
        return TypedResults.Ok();
    }
    private async Task<IResult> GetById(IUnitOfWork uow, Guid id)
    {
        var result = await uow.Shelters.GetByIdAsync(id);
        return TypedResults.Ok(result);
    }

    private async Task<IResult> GetAll(IUnitOfWork uow)
    {
        var result = await uow.Shelters.ListAllAsync();
        return TypedResults.Ok(result);
    }

}