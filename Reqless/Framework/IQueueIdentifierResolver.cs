namespace Reqless.Framework;

/// <summary>
/// Abstraction encapsulating the logic to resolve queue identifiers to queue
/// names.
/// </summary>
public interface IQueueIdentifierResolver
{
    /// <summary>
    /// Resolve the queue identifiers list to their respective concrete queue
    /// names.
    /// </summary>
    /// <param name="queueIdentifiers">The list of queue identifiers to resolve.</param>
    /// <returns>A list of zero or more concrete queue names.</returns>
    public Task<List<string>> ResolveQueueNamesAsync(params string[] queueIdentifiers);
}
