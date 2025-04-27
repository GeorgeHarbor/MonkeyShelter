using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Domain;

namespace MonkeyShelter.Infrastructure;

public static class Seeder
{
    public static async Task SeedAsync(this DataContext context)
    {
        await context.Database.MigrateAsync();

        if (!context.Species.Any())
        {
            var speciesNames = new[]
           {
                "Capuchin", "Howler", "Mandrill", "Spider Monkey", "Golden Lion Tamarin",
                "Squirrel Monkey", "Macaque", "Colobus", "Langur", "Baboon",
                "Tamarin", "Marmoset", "Uakari", "Saki Monkey", "Woolly Monkey"
            };

            var speciesList = speciesNames
                .Select(name => new Species
                {
                    Id = Guid.NewGuid(),
                    Name = name
                })
                .ToList();

            await context.Species.AddRangeAsync(speciesList);
            await context.SaveChangesAsync();
        }
        if (!context.Shelters.Any())
        {
            Shelter shelter = new()
            {
                Id = Guid.NewGuid(),
                Name = "SingularPoint",
                Location = "Belgrade",
                ManagerShelters = []
            };
            context.Shelters.Add(shelter);
            await context.SaveChangesAsync();
        }
        if (!context.Monkeys.Any())
        {
            var rnd = new Random();
            var allSpeciesIds = context.Species.Select(s => s.Id).ToList();
            var monkeys = new List<Monkey>(capacity: 80);

            for (int i = 1; i <= 80; i++)
            {
                var specieId = allSpeciesIds[rnd.Next(allSpeciesIds.Count)];
                var species = context.Species.First(s => s.Id == specieId);
                var shelter = context.Shelters.First();

                monkeys.Add(new Monkey
                {
                    Id = Guid.NewGuid(),
                    Name = $"Monkey{i:D2}",
                    CurrentWeight = (float)Math.Round((decimal)(rnd.NextDouble() * 18 + 2), 1),
                    ArrivalDate = DateTime.UtcNow
                                    .AddDays(-rnd.Next(0, 60)),
                    Species = species,
                    Shelter = shelter
                });
            }

            await context.Monkeys.AddRangeAsync(monkeys);
            await context.SaveChangesAsync();
        }

        if (!context.Arrivals.Any())
        {
            var monkeys = await context.Monkeys.ToListAsync();

            var arrivals = new List<Arrival>();
            foreach (var monkey in monkeys)
            {
                arrivals.Add(new Arrival()
                {
                    Id = Guid.NewGuid(),
                    Monkey = monkey,
                    Date = monkey.ArrivalDate,
                    WeightAtArrival = monkey.CurrentWeight
                });
            }
            await context.Arrivals.AddRangeAsync(arrivals);
            await context.SaveChangesAsync();
        }

        if (!context.VetCheckSchedules.Any())
        {
            var monkeys = await context.Monkeys.ToListAsync();

            var schedules = new List<VetCheckSchedule>();
            foreach (var monkey in monkeys)
            {
                schedules.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Monkey = monkey,
                    ScheduledDate = monkey.ArrivalDate.AddDays(60),
                    CompletedDate = null
                });
            }

            await context.VetCheckSchedules.AddRangeAsync(schedules);
            await context.SaveChangesAsync();
        }

    }
}