using MonkeyShelter.Domain;

namespace MonkeyShelter.Application.Interfaces;

public interface IMonkeyService
{
    /// <summary>
    /// Registers a new monkey arrival, enforcing:
    ///   • max 7 arrivals/day
    ///   • default species & shelter assignment rules
    /// Emits a MonkeyArrived event on success.
    /// </summary>
    Task<Monkey> ArriveMonkeyAsync(ArriveMonkeyRequest req, CancellationToken ct = default);

    /// <summary>
    /// Records a departure, enforcing:
    ///   • max 5 departures/day
    ///   • (departures – arrivals) ≤ 2
    ///   • at least one monkey of that species remains
    /// Emits a MonkeyDeparted event on success.
    /// </summary>
    Task<Monkey> DepartMonkeyAsync(DepartMonkeyRequest req, CancellationToken ct = default);

    /// <summary>
    /// Updates a monkey’s weight, logs the change,
    /// and emits a MonkeyWeightChanged event.
    /// </summary>
    Task<(float, float)> UpdateWeightAsync(UpdateWeightRequest req, CancellationToken ct = default);
}