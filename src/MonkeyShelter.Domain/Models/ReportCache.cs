namespace MonkeyShelter.Domain;

public class ReportCache
{
    public Guid Id { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string Parameters { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string Report { get; set; } = string.Empty;
}