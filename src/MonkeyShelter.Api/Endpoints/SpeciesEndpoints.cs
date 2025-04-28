using Carter;

using MonkeyShelter.Application;

namespace Monkeyspecie.Api;

public class SpeciesEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/species").WithTags("Species");

        grp.MapGet("", GetAll);
        grp.MapGet("/{id}", GetById);
        grp.MapPost("", CreateSpecie).RequireAuthorization();
        grp.MapDelete("", DeleteSpecie).RequireAuthorization();
        grp.MapPut("/{id}", UpdateSpecie).RequireAuthorization();
    }

    private async Task<IResult> UpdateSpecie(IUnitOfWork uow, Guid id, UpdateSpeciesRequest req)
    {
        var specie = await uow.Species.GetByIdAsync(id);

        if (specie is null)
            return TypedResults.NotFound();

        specie.Name = req.Name;
        specie.Description = req.Description;

        uow.Species.Update(specie);
        await uow.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    private async Task<IResult> CreateSpecie(IUnitOfWork uow, CreateSpeciesRequest req)
    {
        var specie = req.MapToSpecie();
        await uow.Species.AddAsync(specie);
        await uow.SaveChangesAsync();
        return TypedResults.Ok();
    }

    private async Task<IResult> DeleteSpecie(IUnitOfWork uow, Guid id)
    {
        var specie = await uow.Species.GetByIdAsync(id);
        if (specie is null)
            return TypedResults.BadRequest("");

        uow.Species.Delete(specie);
        await uow.SaveChangesAsync();
        return TypedResults.Ok();
    }
    private async Task<IResult> GetById(IUnitOfWork uow, Guid id)
    {
        var result = await uow.Species.GetByIdAsync(id);
        return TypedResults.Ok(result);
    }

    private async Task<IResult> GetAll(IUnitOfWork uow)
    {
        var result = await uow.Species.ListAllAsync();
        return TypedResults.Ok(result);
    }

}