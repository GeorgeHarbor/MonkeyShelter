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
            var speciesList = new[]
            {
                new { Name = "Capuchin", Description = "Smart, social monkeys known for tool use." },
                    new { Name = "Howler", Description = "Loud-voiced monkeys with a deep howl." },
                    new { Name = "Mandrill", Description = "Colorful face and rump; largest monkey species." },
                    new { Name = "Spider Monkey", Description = "Long limbs and prehensile tails for swinging." },
                    new { Name = "Golden Lion Tamarin", Description = "Bright orange coat, native to Brazil." },
                    new { Name = "Squirrel Monkey", Description = "Small, agile, and highly active primates." },
                    new { Name = "Macaque", Description = "Widespread, intelligent, adaptable monkeys." },
                    new { Name = "Colobus", Description = "Leaf-eating monkeys with reduced thumbs." },
                    new { Name = "Langur", Description = "Sacred monkeys in India with long tails." },
                    new { Name = "Baboon", Description = "Ground-dwelling monkeys with large canines." },
                    new { Name = "Tamarin", Description = "Small New World monkeys with moustaches." },
                    new { Name = "Marmoset", Description = "Tiny monkeys with claw-like nails." },
                    new { Name = "Uakari", Description = "Red-faced monkeys with short tails." },
                    new { Name = "Saki Monkey", Description = "Shy, arboreal monkeys with bushy tails." },
                    new { Name = "Woolly Monkey", Description = "Strong-bodied monkeys with dense fur." }
            };

            var speciesEntities = speciesList
                .Select(s => new Species
                {
                    Id = Guid.NewGuid(),
                    Name = s.Name,
                    Description = s.Description
                })
            .ToList();

            await context.Species.AddRangeAsync(speciesEntities);
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
            var rnd = new Random(); // declare ONCE before the loop
            foreach (var monkey in monkeys)
            {
                var daysOffset = rnd.Next(0, 60) - 30; // produces -30 to +29
                Console.WriteLine($"Offset: {daysOffset}, ScheduledDate: {DateTime.UtcNow.AddDays(daysOffset)}");

                schedules.Add(new VetCheckSchedule
                {
                    Id = Guid.NewGuid(),
                    Monkey = monkey,
                    ScheduledDate = DateTime.UtcNow.AddDays(daysOffset),
                    CompletedDate = null
                });
            }

            await context.VetCheckSchedules.AddRangeAsync(schedules);
            await context.SaveChangesAsync();
        }

    }
}