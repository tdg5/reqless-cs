namespace Reqless.Framework.QueueIdentifierResolvers;

/// <summary>
/// Abstraction encapsulating the logic to resolve queue identifiers to queue
/// names.
/// </summary>
public interface IQueueIdentifierResolver
{
    /// <summary>
    /// The queue identifiers that should be mapped to their respective concrete
    /// queue names.
    /// </summary>
    public IEnumerable<string> QueueIdentifiers { get; }

    /// <summary>
    /// Resolve the queue identifiers list to their respective concrete queue
    /// names.
    /// </summary>
    /// <returns>A list of zero or more concrete queue names.</returns>
    public Task<List<string>> ResolveQueueNamesAsync();
}