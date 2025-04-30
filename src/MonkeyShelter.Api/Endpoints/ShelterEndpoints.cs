using Carter;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Events;
using MonkeyShelter.Application.Exceptions;

namespace MonkeyShelter.Api;

public class ShelterEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/shelters").WithTags("Shelters");

        grp.MapGet("", GetAll);
        grp.MapGet("/{id}", GetById);
        grp.MapPost("", CreateShelter).RequireAuthorization();
        grp.MapDelete("/{id}", DeleteShelter).RequireAuthorization();
        grp.MapPut("/{id}", UpdateShelter).RequireAuthorization();
    }

    private async Task<IResult> GetAll([FromServices] IUnitOfWork uow, ILogger<ShelterEndpoints> logger)
    {
        try
        {
            var shelters = await uow.Shelters.ListAllAsync();
            if (!shelters.Any())
                return TypedResults.NoContent();

            return TypedResults.Ok(shelters);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all shelters");
            return TypedResults.Problem("An unexpected error occurred.");
        }
    }

    private async Task<IResult> GetById(Guid id, [FromServices] IUnitOfWork uow, ILogger<ShelterEndpoints> logger)
    {
        try
        {
            var shelter = await uow.Shelters.GetByIdAsync(id);
            if (shelter is null)
                return TypedResults.NotFound($"Shelter with ID {id} not found.");

            return TypedResults.Ok(shelter);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving shelter {ShelterId}", id);
            return TypedResults.Problem("An unexpected error occurred.");
        }
    }

    private async Task<IResult> CreateShelter([FromBody] CreateShelterRequest req,
                                              [FromServices] IUnitOfWork uow,
                                              IPublishEndpoint publisher,
                                              ILogger<ShelterEndpoints> logger,
                                              CancellationToken ct)
    {
        try
        {
            var shelter = req.MapToShelter();
            await uow.Shelters.AddAsync(shelter, ct);
            await uow.SaveChangesAsync(ct);

            await publisher.Publish(new ShelterCreated(shelter.Id, shelter.Name, shelter.Location, DateTime.UtcNow),
                ctx => ctx.Headers.Set("MT-Message-Name", nameof(ShelterCreated)), ct);

            return TypedResults.Created($"/shelters/{shelter.Id}", shelter);
        }
        catch (BusinessRuleException ex)
        {
            logger.LogWarning(ex, "Create shelter validation failed: {Error}", ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating shelter");
            return TypedResults.Problem("An unexpected error occurred.");
        }
    }

    private async Task<IResult> DeleteShelter(Guid id,
                                               [FromServices] IUnitOfWork uow,
                                               ILogger<ShelterEndpoints> logger,
                                               CancellationToken ct)
    {
        try
        {
            var shelter = await uow.Shelters.GetByIdAsync(id, ct);
            if (shelter is null)
                return TypedResults.NotFound($"Shelter with ID {id} not found.");

            uow.Shelters.Delete(shelter);
            await uow.SaveChangesAsync(ct);
            return TypedResults.Ok();
        }
        catch (BusinessRuleException ex)
        {
            logger.LogWarning(ex, "Delete shelter validation failed: {Error}", ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error deleting shelter {ShelterId}", id);
            return TypedResults.Problem("An unexpected error occurred.");
        }
    }

    private async Task<IResult> UpdateShelter(Guid id,
                                               [FromBody] UpdateShelterRequest req,
                                               [FromServices] IUnitOfWork uow,
                                               IPublishEndpoint publisher,
                                               ILogger<ShelterEndpoints> logger,
                                               CancellationToken ct)
    {
        try
        {
            var existing = await uow.Shelters.GetByIdAsync(id, ct);
            if (existing is null)
                return TypedResults.NotFound($"Shelter with ID {id} not found.");

            existing.Name = req.Name;
            existing.Location = req.Location;
            uow.Shelters.Update(existing);
            await uow.SaveChangesAsync(ct);

            await publisher.Publish(new ShelterUpdated(existing.Id, existing.Name, existing.Location, DateTime.UtcNow),
                ctx => ctx.Headers.Set("MT-Message-Name", nameof(ShelterUpdated)), ct);

            return TypedResults.NoContent();
        }
        catch (BusinessRuleException ex)
        {
            logger.LogWarning(ex, "Update shelter validation failed: {Error}", ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error updating shelter {ShelterId}", id);
            return TypedResults.Problem("An unexpected error occurred.");
        }
    }
}