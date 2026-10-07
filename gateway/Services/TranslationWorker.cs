using VideoGateway.Models;

namespace VideoGateway.Services;

// Long-running background service: processes one job at a time so uploads never block the web request.
public class TranslationWorker : BackgroundService
{
    private readonly JobQueue _queue;
    private readonly IJobStore _store;
    private readonly FlaskClient _flask;
    private readonly StoragePaths _storage;
    private readonly ILogger<TranslationWorker> _logger;

    public TranslationWorker(JobQueue queue, IJobStore store, FlaskClient flask,
        StoragePaths storage, ILogger<TranslationWorker> logger)
    {
        _queue = queue;
        _store = store;
        _flask = flask;
        _storage = storage;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var jobId in _queue.DequeueAllAsync(stoppingToken))
        {
            var job = _store.Get(jobId);
            if (job is null) continue;

            try
            {
                job.Status = JobStatus.Processing;
                _logger.LogInformation("Job {JobId}: sending to Flask pipeline ({Language})", job.Id, job.Language);

                var flaskFile = await _flask.TranslateAsync(job.InputPath, job.Language, stoppingToken);

                Directory.CreateDirectory(_storage.OutputFolder);
                var localPath = Path.Combine(_storage.OutputFolder, $"{job.Id}.mp4");
                await _flask.DownloadToAsync(flaskFile, localPath, stoppingToken);

                job.OutputPath = localPath;
                job.Status = JobStatus.Done;
                _logger.LogInformation("Job {JobId}: done", job.Id);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                job.Status = JobStatus.Failed;
                job.Error = ex.Message;
                _logger.LogError(ex, "Job {JobId}: failed", job.Id);
            }
            finally
            {
                job.FinishedUtc = DateTime.UtcNow;
                TryDelete(job.InputPath);
            }
        }
    }

    private void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); }
        catch (Exception ex) { _logger.LogWarning(ex, "Could not delete {Path}", path); }
    }
}
