
using Carter;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

using MonkeyShelter.Application;
using MonkeyShelter.Application.Events;
using MonkeyShelter.Application.Exceptions;
using MonkeyShelter.Application.Interfaces;

namespace MonkeyShelter.Api;

public class MonkeyEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/monkeys").WithTags("Monkeys");
        grp.MapGet("", GetAllMonkeys);
        grp.MapGet("/{id}", GetMonkeyById);
        grp.MapPost("/create/arrival", MonkeyArrival).RequireAuthorization();
        grp.MapDelete("/create/departure", MonkeyDeparture).RequireAuthorization();
        grp.MapPut("/update/weight", MonkeyWeightChange).RequireAuthorization();
    }

    private async Task<IResult> MonkeyWeightChange([FromBody] UpdateWeightRequest req, IMonkeyService svc, IPublishEndpoint publisher, ILogger<MonkeyEndpoints> logger, CancellationToken ct)
    {
        try
        {
            (var oldWeight, var newWeight) = await svc.UpdateWeightAsync(req, ct);
            await publisher.Publish(new MonkeyWeightChanged(req.MonkeyId, oldWeight, newWeight, DateTime.UtcNow),
                    ctx => ctx.Headers.Set("MT-Message-Name", nameof(MonkeyWeightChanged)), ct);
            return TypedResults.Ok();
        }
        catch (EntityNotFoundException ex)
        {
            logger.LogError("Bad request: {Error}", ex.Message);
            return TypedResults.BadRequest();
        }
    }

    private async Task<IResult> MonkeyDeparture([FromBody] DepartMonkeyRequest req, [FromServices] IMonkeyService svc, [FromServices] IPublishEndpoint publisher, CancellationToken ct)
    {
        var monkey = await svc.DepartMonkeyAsync(req, ct);

        if (monkey is null)
            return TypedResults.BadRequest();

        await publisher.Publish(new MonkeyDeparted(monkey.Id, monkey.Species.Id, monkey.Shelter.Id, monkey.DepartureDate, monkey.CurrentWeight),
                ctx => ctx.Headers.Set("MT-Message-Name", nameof(MonkeyArrived)), ct);

        var response = monkey.MapToResponse();
        return TypedResults.Created("/create/departure", response);
    }

    private async Task<IResult> MonkeyArrival([FromBody] ArriveMonkeyRequest req, IMonkeyService svc, IPublishEndpoint publisher, CancellationToken ct)
    {
        var monkey = await svc.ArriveMonkeyAsync(req, ct);

        if (monkey is null)
            return TypedResults.BadRequest();

        await publisher.Publish(new MonkeyArrived(monkey.Id, monkey.Species.Id, monkey.Shelter.Id, monkey.ArrivalDate, monkey.CurrentWeight),
                ctx => ctx.Headers.Set("MT-Message-Name", nameof(MonkeyArrived)), ct);

        return TypedResults.Created("/create/arrival", monkey);
    }

    public async Task<IResult> GetAllMonkeys([FromServices] IMonkeyRepository repo)
    {
        var monkeys = await repo.ListWithIncludesAsync(m => m.IsActive);

        if (monkeys.Count == 0)
            return TypedResults.NoContent();
        var dtos = monkeys
            .Select(m => m.MapToResponse())
            .ToList();

        return TypedResults.Ok(dtos);
    }

    public async Task<IResult> GetMonkeyById(Guid id, [FromServices] IMonkeyRepository repo)
    {
        var monkey = await repo.GetByIdWithIncludesAsync(m => m.Id == id);
        if (monkey is null)
            return TypedResults.NotFound();

        var response = monkey.MapToResponse();
        return TypedResults.Ok(response);
    }
}