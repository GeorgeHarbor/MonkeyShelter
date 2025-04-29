namespace MonkeyShelter.Application.Interfaces;

public interface IReportCacheInvalidator
{
    Task InvalidateCountPerSpeciesAsync();
    Task InvalidateArrivalsInRangeAsync(DateTime arrivalDate);
    Task InvalidateAllAsync(DateTime arrivalDate);
}