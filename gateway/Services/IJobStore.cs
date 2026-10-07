using VideoGateway.Models;

namespace VideoGateway.Services;

// Repository abstraction: swap the in-memory store for EF Core later without touching controllers.
public interface IJobStore
{
    void Add(TranslationJob job);
    TranslationJob? Get(Guid id);
    IReadOnlyList<TranslationJob> GetAll();
}
