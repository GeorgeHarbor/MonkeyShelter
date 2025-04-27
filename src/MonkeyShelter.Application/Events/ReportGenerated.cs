namespace MonkeyShelter.Application.Events;

public record ReportGenerated(
        string ReportName,
        string Parameters,
        DateTime GeneratedAt
        );