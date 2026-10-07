namespace VideoGateway.Models;

public enum JobStatus
{
    Pending,
    Processing,
    Done,
    Failed
}

public class TranslationJob
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string OriginalFileName { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
    public string InputPath { get; init; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public string? OutputPath { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
    public DateTime? FinishedUtc { get; set; }
}

// What the API returns to clients (never expose server file paths).
public record JobDto(
    Guid Id,
    string OriginalFileName,
    string Language,
    string Status,
    string? Error,
    DateTime CreatedUtc,
    DateTime? FinishedUtc,
    string? DownloadUrl)
{
    public static JobDto From(TranslationJob job) => new(
        job.Id,
        job.OriginalFileName,
        job.Language,
        job.Status.ToString(),
        job.Error,
        job.CreatedUtc,
        job.FinishedUtc,
        job.Status == JobStatus.Done ? $"/api/jobs/{job.Id}/download" : null);
}
