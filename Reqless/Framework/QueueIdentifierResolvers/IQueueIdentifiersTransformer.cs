namespace Reqless.Framework.QueueIdentifierResolvers;

/// <summary>
/// Abstraction encapsulating the logic to transform semi-sorted queue
/// identifiers to their respective sorted concrete queue names.
/// </summary>
public interface IQueueIdentifiersTransformer
{
    /// <summary>
    /// Transform the given semi-sorted queue identifiers to their respective
    /// concrete queue names.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers that should be
    /// mapped to their respective concrete sorted queue names.</param>
    /// <returns>A list of zero or more sorted concrete queue names.</returns>
    public Task<List<string>> TransformAsync(List<string> queueIdentifiers);
}