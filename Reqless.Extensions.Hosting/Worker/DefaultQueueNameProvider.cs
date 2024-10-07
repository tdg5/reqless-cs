using Reqless.Framework;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Implementation of <see cref="IQueueNameProvider"/> that uses an instance of
/// <see cref="WorkerSettings"/> for queue identifier configuration and
/// an instance of <see cref="IQueueIdentifierResolver"/> for resolving queue
/// identifiers to queue names.
/// </summary>
public class DefaultQueueNameProvider : IQueueNameProvider
{
    private readonly string[] _queueIdentifiers;

    private readonly IQueueIdentifierResolver _queueIdentifierResolver;

    /// <summary>
    /// Create an instance of <see cref="DefaultQueueNameProvider"/>.
    /// </summary>
    /// <param name="settings">The <see cref="IWorkerSettings"/> instance
    /// that should be used for retrieving queue identifiers.</param>
    /// <param name="queueIdentifierResolver">The <see
    /// cref="IQueueIdentifierResolver"/> instance that should be used for
    /// resolving queue identifiers to concrete, prioritized queue
    /// names.</param>
    public DefaultQueueNameProvider(
        IQueueIdentifierResolver queueIdentifierResolver,
        IWorkerSettings settings
    )
    {
        ArgumentNullException.ThrowIfNull(
            queueIdentifierResolver,
            nameof(queueIdentifierResolver)
        );
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));

        _queueIdentifierResolver = queueIdentifierResolver;
        _queueIdentifiers = [.. settings.QueueIdentifiers];
    }

    /// <inheritdoc/>
    public Task<List<string>> GetQueueNamesAsync() =>
        _queueIdentifierResolver.ResolveQueueNamesAsync(_queueIdentifiers);
}
