using MonkeyShelter.Application.Dtos;
using MonkeyShelter.Domain;

namespace MonkeyShelter.Application;

public static class ResponseExtensions
{
    public static MonkeyResponse MapToResponse(this Monkey monkey)
    {
        ArgumentNullException.ThrowIfNull(monkey);

        return new MonkeyResponse(
            monkey.Id.ToString(),
            monkey.Name,
            monkey.CurrentWeight,
            monkey.Species?.Name ?? "<no-species>",
            monkey.Shelter?.Name ?? "<no-shelter>",
            monkey.ArrivalDate,
            monkey.IsActive,
            monkey.CurrentWeight
        );
    }
}