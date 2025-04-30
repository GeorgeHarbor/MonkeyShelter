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
    public class MonkeyEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var grp = app.MapGroup("/monkeys").WithTags("Monkeys");
            grp.MapGet("", GetAllMonkeys);
            grp.MapGet("/{id}", GetMonkeyById);
            grp.MapPost("", MonkeyArrival).RequireAuthorization();
            grp.MapDelete("", MonkeyDeparture).RequireAuthorization();
            grp.MapPut("", MonkeyWeightChange).RequireAuthorization();
        }

        private async Task<IResult> GetAllMonkeys([FromServices] IMonkeyRepository repo, ILogger<MonkeyEndpoints> logger)
        {
            try
            {
                var monkeys = await repo.ListWithIncludesAsync(m => m.IsActive);
                if (!monkeys.Any())
                    return TypedResults.NoContent();

                var dtos = monkeys.Select(m => m.MapToResponse()).ToList();
                return TypedResults.Ok(dtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all monkeys");
                return TypedResults.Problem("An unexpected error occurred.");
            }
        }

        private async Task<IResult> GetMonkeyById(Guid id, [FromServices] IMonkeyRepository repo, ILogger<MonkeyEndpoints> logger)
        {
            try
            {
                var monkey = await repo.GetByIdWithIncludesAsync(m => m.Id == id);
                if (monkey is null)
                    return TypedResults.NotFound($"Monkey with ID {id} not found.");

                return TypedResults.Ok(monkey.MapToResponse());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving monkey {MonkeyId}", id);
                return TypedResults.Problem("An unexpected error occurred.");
            }
        }

        private async Task<IResult> MonkeyArrival([FromBody] ArriveMonkeyRequest req, IMonkeyService svc, IPublishEndpoint publisher, ILogger<MonkeyEndpoints> logger, CancellationToken ct)
        {
            try
            {
                var monkey = await svc.ArriveMonkeyAsync(req, ct);
                await publisher.Publish(new MonkeyArrived(monkey.Id, monkey.Species.Id, monkey.Shelter.Id, monkey.ArrivalDate, monkey.CurrentWeight),
                    ctx => ctx.Headers.Set("MT-Message-Name", nameof(MonkeyArrived)), ct);
                return TypedResults.Created($"/monkeys/{monkey.Id}", monkey.MapToResponse());
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogWarning(ex, "Arrival failed: {Error}", ex.Message);
                return TypedResults.NotFound(ex.Message);
            }
            catch (BusinessRuleException ex)
            {
                logger.LogWarning(ex, "Arrival validation failed: {Error}", ex.Message);
                return TypedResults.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during monkey arrival");
                return TypedResults.Problem("An unexpected error occurred.");
            }
        }

        private async Task<IResult> MonkeyDeparture([FromBody] DepartMonkeyRequest req, IMonkeyService svc, IPublishEndpoint publisher, ILogger<MonkeyEndpoints> logger, CancellationToken ct)
        {
            try
            {
                var monkey = await svc.DepartMonkeyAsync(req, ct);
                await publisher.Publish(new MonkeyDeparted(monkey.Id, monkey.Species.Id, monkey.Shelter.Id, monkey.DepartureDate, monkey.CurrentWeight),
                    ctx => ctx.Headers.Set("MT-Message-Name", nameof(MonkeyDeparted)), ct);
                return TypedResults.Ok(monkey.MapToResponse());
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogWarning(ex, "Departure failed: {Error}", ex.Message);
                return TypedResults.NotFound(ex.Message);
            }
            catch (BusinessRuleException ex)
            {
                logger.LogWarning(ex, "Departure validation failed: {Error}", ex.Message);
                return TypedResults.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during monkey departure");
                return TypedResults.Problem("An unexpected error occurred.");
            }
        }

        private async Task<IResult> MonkeyWeightChange([FromBody] UpdateWeightRequest req, IMonkeyService svc, IPublishEndpoint publisher, ILogger<MonkeyEndpoints> logger, CancellationToken ct)
        {
            try
            {
                var (oldWeight, newWeight) = await svc.UpdateWeightAsync(req, ct);
                await publisher.Publish(new MonkeyWeightChanged(req.MonkeyId, oldWeight, newWeight, DateTime.UtcNow),
                    ctx => ctx.Headers.Set("MT-Message-Name", nameof(MonkeyWeightChanged)), ct);
                return TypedResults.Ok();
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogWarning(ex, "Weight update failed: {Error}", ex.Message);
                return TypedResults.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during weight update");
                return TypedResults.Problem("An unexpected error occurred.");
            }
        }
    }
}