using System.Threading.Channels;

namespace VideoGateway.Services;

// Producer/consumer queue: controllers enqueue job ids, the background worker dequeues them.
public class JobQueue
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>();

    public ValueTask EnqueueAsync(Guid jobId) => _channel.Writer.WriteAsync(jobId);

    public IAsyncEnumerable<Guid> DequeueAllAsync(CancellationToken ct) =>
        _channel.Reader.ReadAllAsync(ct);
}
