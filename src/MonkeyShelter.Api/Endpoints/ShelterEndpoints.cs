using Carter;

using MonkeyShelter.Application;
using MonkeyShelter.Infrastructure;

namespace MonkeyShelter.Api;

public class ShelterEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/shelters").WithTags("Shelters");

        grp.MapGet("", GetAll);
        grp.MapGet("/{id}", GetById);
        grp.MapPost("", CreateShelter);
        grp.MapDelete("", DeleteShelter);
        grp.MapPut("/{id}", UpdateShelter);
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

    private async Task<IResult> CreateShelter(IUnitOfWork uow, CreateShelterRequest req)
    {
        var shelter = req.MapToShelter();
        await uow.Shelters.AddAsync(shelter);
        await uow.SaveChangesAsync();
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