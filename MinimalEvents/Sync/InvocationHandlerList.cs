using System.Collections.Immutable;

namespace OpenShock.MinimalEvents.Sync;

internal sealed class InvocationHandlerList<T>
{
    public ImmutableArray<T> Handlers { get; private set; } = ImmutableArray<T>.Empty;
    
#if NETSTANDARD2_1
    private readonly object _lock = new object();
#else
    private readonly Lock _lock = new();
#endif

    public void Add(T handler)
    {
        lock (_lock)
        {
            Handlers = Handlers.Add(handler);
        }
    }

    public void Remove(T handler)
    {
        lock (_lock)
        {
            Handlers = Handlers.Remove(handler);
        }
    }
}