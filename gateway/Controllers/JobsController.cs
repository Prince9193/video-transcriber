using Microsoft.AspNetCore.Mvc;
using VideoGateway.Models;
using VideoGateway.Services;

namespace VideoGateway.Controllers;

[ApiController]
[Route("api")]
public class JobsController : ControllerBase
{
    private static readonly string[] AllowedExtensions = { ".mp4", ".avi", ".mov", ".wmv" };

    private readonly IJobStore _store;
    private readonly JobQueue _queue;
    private readonly FlaskClient _flask;
    private readonly StoragePaths _storage;

    public JobsController(IJobStore store, JobQueue queue, FlaskClient flask, StoragePaths storage)
    {
        _store = store;
        _queue = queue;
        _flask = flask;
        _storage = storage;
    }

    // POST /api/jobs  (multipart: video + language) -> 202 Accepted with a job to poll
    [HttpPost("jobs")]
    public async Task<IActionResult> Create([FromForm] IFormFile? video, [FromForm] string? language)
    {
        if (video is null || video.Length == 0)
            return BadRequest(new { message = "No video file provided" });
        if (string.IsNullOrWhiteSpace(language))
            return BadRequest(new { message = "Language is required" });

        var maxBytes = _config.GetValue("Storage:MaxUploadMegabytes", 100) * 1024L * 1024L;
        if (video.Length > maxBytes)
            return BadRequest(new { message = $"File exceeds {maxBytes / 1024 / 1024} MB limit" });

        var extension = Path.GetExtension(video.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return BadRequest(new { message = "Unsupported format. Use MP4, AVI, MOV or WMV." });

        Directory.CreateDirectory(_storage.UploadFolder);

        var id = Guid.NewGuid();
        var inputPath = Path.Combine(_storage.UploadFolder, $"{id}{extension}");
        await using (var target = System.IO.File.Create(inputPath))
        {
            await video.CopyToAsync(target);
        }

        var job = new TranslationJob
        {
            Id = id,
            OriginalFileName = Path.GetFileName(video.FileName),
            Language = language,
            InputPath = inputPath
        };
        _store.Add(job);
        await _queue.EnqueueAsync(job.Id);

        return AcceptedAtAction(nameof(GetById), new { id = job.Id }, JobDto.From(job));
    }

    // GET /api/jobs/{id}  -> status polling
    [HttpGet("jobs/{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var job = _store.Get(id);
        return job is null ? NotFound(new { message = "Job not found" }) : Ok(JobDto.From(job));
    }

    // GET /api/jobs  -> history
    [HttpGet("jobs")]
    public IActionResult GetAll() => Ok(_store.GetAll().Select(JobDto.From));

    // GET /api/jobs/{id}/download
    [HttpGet("jobs/{id:guid}/download")]
    public IActionResult Download(Guid id)
    {
        var job = _store.Get(id);
        if (job is null) return NotFound(new { message = "Job not found" });
        if (job.Status != JobStatus.Done || job.OutputPath is null || !System.IO.File.Exists(job.OutputPath))
            return Conflict(new { message = $"Job is {job.Status}; output not available" });

        var downloadName = $"translated_{Path.GetFileNameWithoutExtension(job.OriginalFileName)}.mp4";
        return PhysicalFile(Path.GetFullPath(job.OutputPath), "video/mp4", downloadName);
    }

    // GET /api/languages -> proxied from the Flask service so the list stays in one place
    [HttpGet("languages")]
    public async Task<IActionResult> Languages(CancellationToken ct)
    {
        try
        {
            var json = await _flask.GetLanguagesJsonAsync(ct);
            return Content(json, "application/json");
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status502BadGateway,
                new { message = "Translation service (Flask) is not reachable" });
        }
    }
}
