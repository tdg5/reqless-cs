using System.Collections.ObjectModel;
using Reqless.Framework;

namespace Reqless.Worker;

/// <summary>
/// Implementation of <see cref="IQueueNameProvider"/> that uses an instance of
/// <see cref="ReqlessWorkerSettings"/> for queue identifier configuration and
/// an instance of <see cref="IQueueIdentifierResolver"/> for resolving queue
/// identifiers to queue names.
/// </summary>
public class DefaultQueueNameProvider : IQueueNameProvider
{
    private readonly IReqlessWorkerSettings _settings;

    private readonly IQueueIdentifierResolver _queueIdentifierResolver;

    private string[]? _cachedQueueIdentifiers = null;

    private WeakReference<ReadOnlyCollection<string>> _cachedQueueIdentifiersSource;

    /// <summary>
    /// Create an instance of <see cref="DefaultQueueNameProvider"/>.
    /// </summary>
    /// <param name="settings">The <see cref="IReqlessWorkerSettings"/> instance
    /// that should be used for retrieving queue identifiers.</param>
    /// <param name="queueIdentifierResolver">The <see
    /// cref="IQueueIdentifierResolver"/> instance that should be used for
    /// resolving queue identifiers to concrete, prioritized queue
    /// names.</param>
    public DefaultQueueNameProvider(
        IQueueIdentifierResolver queueIdentifierResolver,
        IReqlessWorkerSettings settings
    )
    {
        _queueIdentifierResolver = queueIdentifierResolver;
        _settings = settings;
        _cachedQueueIdentifiersSource = new(settings.QueueIdentifiers);
    }

    /// <inheritdoc/>
    public Task<List<string>> GetQueueNamesAsync()
    {
        _cachedQueueIdentifiersSource.TryGetTarget(out var cachedQueueIdentifiersSource);
        if (
            _cachedQueueIdentifiers is null
            || cachedQueueIdentifiersSource != _settings.QueueIdentifiers
        )
        {
            _cachedQueueIdentifiersSource = new(_settings.QueueIdentifiers);
            _cachedQueueIdentifiers = [.. _settings.QueueIdentifiers];
        }
        return _queueIdentifierResolver.ResolveQueueNamesAsync(_cachedQueueIdentifiers);
    }
}
