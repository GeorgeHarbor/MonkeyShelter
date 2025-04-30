using FluentAssertions;
using Moq;
using MonkeyShelter.Application.Exceptions;
using MonkeyShelter.Application.Interfaces;
using MonkeyShelter.Domain;
using System.Linq.Expressions;

namespace MonkeyShelter.Application.Tests.Unit;

public class MonkeyServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMonkeyRepository> _repoMock;
    private readonly MonkeyService _service;

    public MonkeyServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _repoMock = new Mock<IMonkeyRepository>();
        _service = new MonkeyService(_uowMock.Object, _repoMock.Object);
    }

    [Fact]
    public async Task ArriveMonkeyAsync_UnderDailyLimit_AddsMonkeyAndArrival()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var req = new ArriveMonkeyRequest
        (Guid.NewGuid(), Guid.NewGuid(), "George", 5.5f, today);

        _uowMock.Setup(u => u.Arrivals.CountAsync(
                    It.IsAny<Expression<Func<Arrival, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        var species = new Species { Id = req.SpeciesId, Name = "Chimp" };
        _uowMock.Setup(u => u.Species.GetByIdAsync(req.SpeciesId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(species);
        var shelter = new Shelter { Id = req.ShelterId, Name = "Main" };
        _uowMock.Setup(u => u.Shelters.GetByIdAsync(req.ShelterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shelter);
        _repoMock.Setup(r => r.ListWithIncludesAsync(It.IsAny<Expression<Func<Monkey, bool>>>()))
            .ReturnsAsync([]);
        _uowMock.Setup(u => u.Monkeys.AddAsync(It.IsAny<Monkey>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.Arrivals.AddAsync(It.IsAny<Arrival>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.ArriveMonkeyAsync(req);

        // Assert
        result.Name.Should().Be(req.Name);
        result.Species.Id.Should().Be(req.SpeciesId);
        result.Shelter.Id.Should().Be(req.ShelterId);
        result.CurrentWeight.Should().Be(req.Weight);
        _uowMock.Verify(u => u.Monkeys.AddAsync(It.IsAny<Monkey>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.Arrivals.AddAsync(It.IsAny<Arrival>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ArriveMonkeyAsync_DailyLimitReached_ThrowsBusinessRuleException()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var req = new ArriveMonkeyRequest
        (Guid.NewGuid(), Guid.NewGuid(), "George", 5.5f, today);
        _uowMock.Setup(u => u.Arrivals.CountAsync(
                It.IsAny<Expression<Func<Arrival, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(7);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => _service.ArriveMonkeyAsync(req));
    }

    [Fact]
    public async Task ArriveMonkeyAsync_SpeciesNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var req = new ArriveMonkeyRequest
        (Guid.NewGuid(), Guid.NewGuid(), "George", 5.5f, today);

        _uowMock.Setup(u => u.Arrivals.CountAsync(
                It.IsAny<Expression<Func<Arrival, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _uowMock.Setup(u => u.Species.GetByIdAsync(req.SpeciesId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Species)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.ArriveMonkeyAsync(req));
    }

    [Fact]
    public async Task DepartMonkeyAsync_UnderLimits_UpdatesMonkeyAndRecordsDeparture()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var monkeyId = Guid.NewGuid();
        var req = new DepartMonkeyRequest(monkeyId, today, 7.2f);
        _uowMock.Setup(u => u.Departures.CountAsync(It.IsAny<Expression<Func<Departure, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _uowMock.Setup(u => u.Arrivals.CountAsync(It.IsAny<Expression<Func<Arrival, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        var existing = new Monkey { Id = monkeyId, Name = "George", Shelter = new Shelter { Id = Guid.NewGuid() }, Species = new Species { Id = Guid.NewGuid() }, CurrentWeight = 6.0f };
        _repoMock.Setup(r => r.GetByIdWithIncludesAsync(It.IsAny<Expression<Func<Monkey, bool>>>()))
            .ReturnsAsync(existing);
        _repoMock.Setup(r => r.ListWithIncludesAsync(It.IsAny<Expression<Func<Monkey, bool>>>()))
            .ReturnsAsync([existing, existing]);
        _uowMock.Setup(u => u.Departures.AddAsync(It.IsAny<Departure>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.Monkeys.Update(It.IsAny<Monkey>()));
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.DepartMonkeyAsync(req);

        // Assert
        result.IsActive.Should().BeFalse();
        result.CurrentWeight.Should().Be(req.WeightAtDeparture);
        result.DepartureDate.Should().Be(today);
        _uowMock.Verify(u => u.Departures.AddAsync(It.IsAny<Departure>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.Monkeys.Update(It.IsAny<Monkey>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateWeightAsync_NonexistentMonkey_ThrowsEntityNotFoundException()
    {
        // Arrange
        var req = new UpdateWeightRequest(Guid.NewGuid(), 5.5f);
        _uowMock.Setup(u => u.Monkeys.GetByIdAsync(req.MonkeyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Monkey)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateWeightAsync(req));
    }

    [Fact]
    public async Task UpdateWeightAsync_ExistingMonkey_UpdatesWeight()
    {
        // Arrange
        var monkey = new Monkey { Id = Guid.NewGuid(), CurrentWeight = 4.4f, Name = "George", Shelter = new(), Species = new() };
        var req = new UpdateWeightRequest(monkey.Id, 5.5f);
        _uowMock.Setup(u => u.Monkeys.GetByIdAsync(monkey.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(monkey);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var (oldWeight, newWeight) = await _service.UpdateWeightAsync(req);

        // Assert
        oldWeight.Should().Be(4.4f);
        newWeight.Should().Be(5.5f);
        monkey.CurrentWeight.Should().Be(5.5f);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}