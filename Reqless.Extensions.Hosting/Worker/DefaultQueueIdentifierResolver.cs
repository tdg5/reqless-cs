using Reqless.Client;
using Reqless.Common.Validation;
using Reqless.Framework;
using Reqless.Framework.QueueIdentifierResolvers;

namespace Reqless.Extensions.Hosting.Worker;

/// <summary>
/// Default implementation of <see cref="IQueueIdentifierResolver"/> that
/// handles mapping dynamic queue identifiers to concrete queue names and then
/// sorts them based on dynamic queue priorities.
/// </summary>
public class DefaultQueueIdentifierResolver : IQueueIdentifierResolver
{
    private readonly TransformingQueueIdentifierResolver _innerResolver;

    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="DefaultQueueIdentifierResolver"/> class.
    /// </summary>
    /// <param name="reqlessClient">The <see cref="IReqlessClient"/> instance to
    /// use for making requests to Reqless.</param>
    public DefaultQueueIdentifierResolver(IReqlessClient reqlessClient)
    {
        ArgumentNullException.ThrowIfNull(reqlessClient, nameof(reqlessClient));

        _innerResolver = new([
            new DynamicMappingQueueIdentifiersTransformer(reqlessClient),
            new DynamicPriorityQueueIdentifiersTransformer(reqlessClient),
        ]);
    }

    /// <inheritdoc/>
    public Task<List<string>> ResolveQueueNamesAsync(params string[] queueIdentifiers)
    {
        ArgumentValidation.ThrowIfAnyNullOrWhitespace(
            queueIdentifiers, nameof(queueIdentifiers));

        return _innerResolver.ResolveQueueNamesAsync(queueIdentifiers);
    }
}
