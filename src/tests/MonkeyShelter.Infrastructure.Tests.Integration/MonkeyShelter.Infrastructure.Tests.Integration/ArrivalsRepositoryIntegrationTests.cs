using FluentAssertions;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Domain;

namespace MonkeyShelter.Infrastructure.Tests.Integration;

public class ArrivalsRepositoryIntegrationTests : IAsyncLifetime
{
    private SqliteConnection? _connection;
    private DataContext? _context;
    private ArrivalsRepository? _repository;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DataContext(options);
        await _context.Database.EnsureCreatedAsync();

        _repository = new ArrivalsRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        _context.Dispose();
    }

    [Fact]
    public async Task AddAndGetByIdAsync_ShouldPersistAndRetrieveArrival()
    {
        // --- Arrange ---
        var species = new Species { Id = Guid.NewGuid(), Name = "Chimpanzee" };
        var shelter = new Shelter { Id = Guid.NewGuid(), Name = "Main Shelter", Location = "Test Location" };
        var monkey = new Monkey
        {
            Id = Guid.NewGuid(),
            Name = "TestMonkey",
            Species = species,
            Shelter = shelter,
            CurrentWeight = 10,
            ArrivalDate = DateTime.UtcNow.Date
        };

        _context.Species.Add(species);
        _context.Shelters.Add(shelter);
        _context.Monkeys.Add(monkey);
        await _context.SaveChangesAsync();

        var arrival = new Arrival
        {
            Id = Guid.NewGuid(),
            Monkey = monkey,
            Date = monkey.ArrivalDate,
            WeightAtArrival = monkey.CurrentWeight
        };

        // --- Act ---
        await _repository.AddAsync(arrival);
        await _context.SaveChangesAsync();

        var fetched = await _repository.GetByIdAsync(arrival.Id);

        // --- Assert ---
        fetched.Should().NotBeNull();
        fetched!.WeightAtArrival.Should().Be(arrival.WeightAtArrival);
        fetched.Monkey.Id.Should().Be(monkey.Id);
    }
}