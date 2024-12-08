namespace Reqless.Framework.QueueIdentifierResolvers;

/// <summary>
/// Implementation of <see cref="IQueueIdentifierResolver"/> that resolves queue
/// identifiers using a series of transformation operations.
/// </summary>
public class TransformingQueueIdentifierResolver : IQueueIdentifierResolver
{
    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="TransformingQueueIdentifierResolver"/> class.
    /// </summary>
    /// <param name="queueIdentifiersTransformers">The <see
    /// cref="IQueueIdentifierResolver"/> instances to chain together when
    /// resolving queue identifiers.</param>
    public TransformingQueueIdentifierResolver(
        IEnumerable<IQueueIdentifiersTransformer> queueIdentifiersTransformers)
    {
        ArgumentNullException.ThrowIfNull(
            queueIdentifiersTransformers, nameof(queueIdentifiersTransformers));
        QueueIdentifiersTransformers = queueIdentifiersTransformers;
    }

    /// <summary>
    /// Gets the <see cref="IQueueIdentifiersTransformer"/> instances to chain
    /// together when resolving queue identifiers.
    /// </summary>
    protected IEnumerable<IQueueIdentifiersTransformer> QueueIdentifiersTransformers { get; }

    /// <inheritdoc />
    public async Task<List<string>> ResolveQueueNamesAsync(params string[] queueIdentifiers)
    {
        ArgumentNullException.ThrowIfNull(queueIdentifiers, nameof(queueIdentifiers));
        List<string> resolvedQueueNames = [.. queueIdentifiers];
        foreach (var queueIdentifiersTransformer in QueueIdentifiersTransformers)
        {
            resolvedQueueNames =
                await queueIdentifiersTransformer.TransformAsync(resolvedQueueNames);
        }

        return resolvedQueueNames;
    }
}
