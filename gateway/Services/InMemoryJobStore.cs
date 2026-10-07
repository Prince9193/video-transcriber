using System.Collections.Concurrent;
using VideoGateway.Models;

namespace VideoGateway.Services;

public class InMemoryJobStore : IJobStore
{
    private readonly ConcurrentDictionary<Guid, TranslationJob> _jobs = new();

    public void Add(TranslationJob job) => _jobs[job.Id] = job;

    public TranslationJob? Get(Guid id) => _jobs.TryGetValue(id, out var job) ? job : null;

    public IReadOnlyList<TranslationJob> GetAll() =>
        _jobs.Values.OrderByDescending(j => j.CreatedUtc).ToList();
}
