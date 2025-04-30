using System;

using Carter;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Events;
using MonkeyShelter.Application.Exceptions;
using MonkeyShelter.Application.Interfaces;

namespace MonkeyShelter.Api
{
    public class SpeciesEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var grp = app.MapGroup("/species").WithTags("Species");

            grp.MapGet("", GetAll);
            grp.MapGet("/{id}", GetById);
            grp.MapPost("", CreateSpecies).RequireAuthorization();
            grp.MapDelete("/{id}", DeleteSpecies).RequireAuthorization();
            grp.MapPut("/{id}", UpdateSpecies).RequireAuthorization();
        }

        private async Task<IResult> GetAll([FromServices] IUnitOfWork uow, ILogger<SpeciesEndpoints> logger)
        {
            try
            {
                var list = await uow.Species.ListAllAsync();
                if (!list.Any())
                    return TypedResults.NoContent();

                var dtos = list.Select(s => s).ToList();
                return TypedResults.Ok(dtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving species list");
                return TypedResults.Problem("An unexpected error occurred.");
            }
        }

        private async Task<IResult> GetById(Guid id, [FromServices] IUnitOfWork uow, ILogger<SpeciesEndpoints> logger)
        {
            try
            {
                var specie = await uow.Species.GetByIdAsync(id);
                if (specie is null)
                    return TypedResults.NotFound($"Species with ID {id} not found.");

                return TypedResults.Ok(specie);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving species {SpeciesId}", id);
                return TypedResults.Problem("An unexpected error occurred.");
            }
        }

        private async Task<IResult> CreateSpecies(
            [FromBody] CreateSpeciesRequest req,
            [FromServices] IUnitOfWork uow,
            IPublishEndpoint publisher,
            ILogger<SpeciesEndpoints> logger,
            CancellationToken ct)
        {
            try
            {
                var specie = req.MapToSpecie();
                await uow.Species.AddAsync(specie, ct);
                await uow.SaveChangesAsync(ct);

                await publisher.Publish(
                    new SpeciesCreated(specie.Id, specie.Name, specie.Description, DateTime.UtcNow),
                    ctx => ctx.Headers.Set("MT-Message-Name", nameof(SpeciesCreated)), ct);

                return TypedResults.Created($"/species/{specie.Id}", specie);
            }
            catch (BusinessRuleException ex)
            {
                logger.LogWarning(ex, "Create species validation failed: {Error}", ex.Message);
                return TypedResults.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error creating species");
                return TypedResults.Problem("An unexpected error occurred.");
            }
        }

        private async Task<IResult> UpdateSpecies(
            Guid id,
            [FromBody] UpdateSpeciesRequest req,
            [FromServices] IUnitOfWork uow,
            IPublishEndpoint publisher,
            ILogger<SpeciesEndpoints> logger,
            CancellationToken ct)
        {
            try
            {
                var existing = await uow.Species.GetByIdAsync(id);
                if (existing is null)
                    return TypedResults.NotFound($"Species with ID {id} not found.");

                existing.Name = req.Name;
                existing.Description = req.Description;
                uow.Species.Update(existing);
                await uow.SaveChangesAsync(ct);

                await publisher.Publish(
                    new SpeciesUpdated(existing.Id, existing.Name, existing.Description, DateTime.UtcNow),
                    ctx => ctx.Headers.Set("MT-Message-Name", nameof(SpeciesUpdated)), ct);

                return TypedResults.NoContent();
            }
            catch (BusinessRuleException ex)
            {
                logger.LogWarning(ex, "Update species validation failed: {Error}", ex.Message);
                return TypedResults.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error updating species {SpeciesId}", id);
                return TypedResults.Problem("An unexpected error occurred.");
            }
        }

        private async Task<IResult> DeleteSpecies(
            Guid id,
            [FromServices] IUnitOfWork uow,
            ILogger<SpeciesEndpoints> logger,
            CancellationToken ct)
        {
            try
            {
                var existing = await uow.Species.GetByIdAsync(id);
                if (existing is null)
                    return TypedResults.NotFound($"Species with ID {id} not found.");

                uow.Species.Delete(existing);
                await uow.SaveChangesAsync(ct);
                return TypedResults.Ok();
            }
            catch (BusinessRuleException ex)
            {
                logger.LogWarning(ex, "Delete species validation failed: {Error}", ex.Message);
                return TypedResults.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error deleting species {SpeciesId}", id);
                return TypedResults.Problem("An unexpected error occurred.");
            }
        }
    }
}