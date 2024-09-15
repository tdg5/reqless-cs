namespace Reqless.Framework.QueueIdentifierResolvers;

/// <summary>
/// Implementation of <see cref="IQueueIdentifierResolver"/> that resolves queue
/// identifiers using a series of transformation operations.
/// </summary>
public class TransformingQueueIdentifierResolver : IQueueIdentifierResolver
{
    /// <inheritdoc />
    public IEnumerable<string> QueueIdentifiers { get; }

    /// <summary>
    /// The <see cref="IQueueIdentifiersTransformer"/> instances to chain
    /// together when resolving queue identifiers.
    /// </summary>
    protected IEnumerable<IQueueIdentifiersTransformer> QueueIdentifiersTransformers { get; }

    /// <summary>
    /// Create an instance of <see cref="TransformingQueueIdentifierResolver"/>.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers that should be
    /// mapped to their respective concrete queue names.</param>
    /// <param name="queueIdentifiersTransformers">The <see
    /// cref="IQueueIdentifierResolver"/> instances to chain together when
    /// resolving queue identifiers.</param>
    public TransformingQueueIdentifierResolver(
        IEnumerable<string> queueIdentifiers,
        IEnumerable<IQueueIdentifiersTransformer> queueIdentifiersTransformers
    )
    {
        ArgumentNullException.ThrowIfNull(queueIdentifiers, nameof(queueIdentifiers));
        ArgumentNullException.ThrowIfNull(
            queueIdentifiersTransformers,
            nameof(queueIdentifiersTransformers)
        );
        QueueIdentifiers = queueIdentifiers;
        QueueIdentifiersTransformers = queueIdentifiersTransformers;
    }

    /// <inheritdoc />
    public async Task<List<string>> ResolveQueueNamesAsync()
    {
        List<string> resolvedQueueNames = [.. QueueIdentifiers];
        foreach (var queueIdentifiersTransformer in QueueIdentifiersTransformers)
        {
            resolvedQueueNames =
                await queueIdentifiersTransformer.TransformAsync(resolvedQueueNames);
        }
        return resolvedQueueNames;
    }
}
