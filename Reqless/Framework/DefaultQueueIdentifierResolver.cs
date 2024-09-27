using Reqless.Client;
using Reqless.Framework.QueueIdentifierResolvers;

namespace Reqless.Framework;

/// <summary>
/// Default implementation of <see cref="IQueueIdentifierResolver"/> that
/// handles mapping dynamic queue identifiers to concrete queue names and then
/// sorts them based on dynamic queue priorities.
/// </summary>
public class DefaultQueueIdentifierResolver : IQueueIdentifierResolver
{
    private readonly TransformingQueueIdentifierResolver _innerResolver;

    /// <summary>
    /// Create an instance of <see cref="DefaultQueueIdentifierResolver"/>.
    /// </summary>
    /// <param name="reqlessClient">The <see cref="IReqlessClient"/> instance to
    /// use for making requests to Reqless.</param>
    public DefaultQueueIdentifierResolver(
        IReqlessClient reqlessClient
    )
    {
        ArgumentNullException.ThrowIfNull(reqlessClient, nameof(reqlessClient));

        _innerResolver = new([
            new DynamicMappingQueueIdentifiersTransformer(reqlessClient),
            new DynamicPriorityQueueIdentifiersTransformer(reqlessClient),
        ]);
    }

    /// <inheritdoc />
    public Task<List<string>> ResolveQueueNamesAsync(params string[] queueIdentifiers)
    {
        return _innerResolver.ResolveQueueNamesAsync(queueIdentifiers);
    }
}