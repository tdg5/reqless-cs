using Reqless.Client;
using Reqless.Common.Validation;
using System.Text.RegularExpressions;

namespace Reqless.Framework.QueueIdentifierResolvers;

/// <summary>
/// Implementation of <see cref="IQueueIdentifiersTransformer"/> that
/// transforms queue identifiers by replacing dynamic queue identifier patterns
/// with conrete queue names matching those patterns.
/// </summary>
public class DynamicMappingQueueIdentifiersTransformer : IQueueIdentifiersTransformer
{
    /// <summary>
    /// The <see cref="IReqlessClient"/> instance used to fetch queue identifier
    /// patterns.
    /// </summary>
    protected IReqlessClient ReqlessClient { get; }

    /// <summary>
    /// The time-to-live in milliseconds for the queue identifier patterns cache.
    /// </summary>
    protected int CacheTtlMilliseconds { get; }

    private long _cacheExpiresAt = 0;

    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    private Dictionary<string, List<string>> _queueIdentifierPatterns = [];

    /// <summary>
    /// Create an instance of <see
    /// cref="DynamicMappingQueueIdentifiersTransformer"/>.
    /// </summary>
    /// <param name="reqlessClient">The <see cref="IReqlessClient"/> instance used to
    /// fetch queue identifer patterns.</param>
    /// <param name="cacheTtlMilliseconds">The time-to-live in milliseconds for
    /// the queue identifer patterns cache.</param>
    public DynamicMappingQueueIdentifiersTransformer(
        IReqlessClient reqlessClient,
        int cacheTtlMilliseconds = 60000
    )
    {
        ArgumentNullException.ThrowIfNull(reqlessClient, nameof(reqlessClient));
        ArgumentValidation.ThrowIfNegative(cacheTtlMilliseconds, nameof(cacheTtlMilliseconds));

        ReqlessClient = reqlessClient;
        CacheTtlMilliseconds = cacheTtlMilliseconds;
    }

    /// <summary>
    /// Transform the given queue identifiers by replacing dynamic queue
    /// identifier patterns with the concrete queue names matching those
    /// patterns as defined by <see
    /// cref="IReqlessClient.GetAllQueueIdentifierPatternsAsync"/>.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers that should be
    /// mapped to their respective concrete queue names.</param>
    public async Task<List<string>> TransformAsync(List<string> queueIdentifiers)
    {
        var queueIdentifierPatterns = await GetQueueIdentifierPatternsAsync();
        var knownQueueNames = await ReqlessClient.GetAllQueueNamesAsync();
        List<string> expandedQueueIdentifiers = MapQueueIdentifiers(
            queueIdentifiers,
            queueIdentifierPatterns
        );
        List<string> mappedQueueIdentifiers = MatchPatternsToKnownQueues(
            expandedQueueIdentifiers,
            knownQueueNames
        );
        return mappedQueueIdentifiers;
    }

    /// <summary>
    /// Get the queue identifier patterns either from the in-memory cache or by
    /// fetching the patterns from the Reqless backend.
    /// </summary>
    /// <returns>The mapping of dynamic queue identifiers to concrete queue names.</returns>
    protected async Task<Dictionary<string, List<string>>> GetQueueIdentifierPatternsAsync()
    {
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (now >= _cacheExpiresAt)
        {
            await _cacheLock.WaitAsync();
            try
            {
                // Make sure another caller hasn't already refreshed the cache.
                if (now >= _cacheExpiresAt)
                {
                    _queueIdentifierPatterns = await ReqlessClient.GetAllQueueIdentifierPatternsAsync();
                    _cacheExpiresAt = now + CacheTtlMilliseconds;
                }
            }
            finally
            {
                _cacheLock.Release();
            }
        }
        return _queueIdentifierPatterns;
    }

    /// <summary>
    /// Map the given queue identifiers to an expanded set of queue identifiers
    /// based on the queue identifier patterns.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers to map and/or
    /// expand.</param>
    /// <param name="queueIdentifierPatterns">The queue identifier patterns
    /// defining the mapping from dynamic queue identifiers to concrete queue
    /// names.</param>
    /// <returns>The mapped queue identifiers.</returns>
    protected static List<string> MapQueueIdentifiers(
        List<string> queueIdentifiers,
        Dictionary<string, List<string>> queueIdentifierPatterns
    )
    {
        List<string> expandedQueueIdentifiers = [];
        foreach (string queueIdentifier in queueIdentifiers)
        {
            bool negated = queueIdentifier.StartsWith('!');
            string _queueIdentifier = negated ? queueIdentifier[1..] : queueIdentifier;
            if (!_queueIdentifier.StartsWith('@'))
            {
                expandedQueueIdentifiers.Add(queueIdentifier);
                continue;
            }

            var mappedQueueIdentifiers = queueIdentifierPatterns.GetValueOrDefault(
                _queueIdentifier[1..],
                []
            );
            foreach (string mappedQueueIdentifier in mappedQueueIdentifiers)
            {
                string _pattern = !negated
                    ? mappedQueueIdentifier
                    : mappedQueueIdentifier.StartsWith('!')
                        ? mappedQueueIdentifier[1..]
                        : "!" + mappedQueueIdentifier;
                expandedQueueIdentifiers.Add(_pattern);
            }
        }
        return expandedQueueIdentifiers;
    }

    /// <summary>
    /// Match the given expanded queue identifiers to known queue names.
    /// </summary>
    /// <param name="queueIdentifiers">Expanded queue identifiers that should be
    /// resolved against real, known queue names.</param>
    /// <param name="knownQueueNames">The names of real, known queues.</param>
    /// <returns>The list of zero or more real, known queues that matched the
    /// given queue identifiers.</returns>
    protected static List<string> MatchPatternsToKnownQueues(
        List<string> queueIdentifiers,
        List<string> knownQueueNames
    )
    {
        List<string> matchedQueueIdentifiers = [];
        foreach (var queueIdentifier in queueIdentifiers)
        {
            bool isStaticPattern = !(
                queueIdentifier.Contains('!') || queueIdentifier.Contains('*')
            );
            // Always include static patterns even if a real queue doesn't exist.
            if (isStaticPattern && !matchedQueueIdentifiers.Contains(queueIdentifier))
            {
                matchedQueueIdentifiers.Add(queueIdentifier);
                continue;
            }

            bool negated = queueIdentifier.StartsWith('!');
            string pattern = (
                negated ? queueIdentifier[1..] : queueIdentifier
            ).Replace("*", ".*");
            string anchoredPattern = $"^{pattern}$";
            foreach (string knownQueueName in knownQueueNames)
            {
                if (!Regex.IsMatch(knownQueueName, anchoredPattern))
                {
                    continue;
                }

                if (negated)
                {
                    matchedQueueIdentifiers.Remove(knownQueueName);
                }
                else if (!matchedQueueIdentifiers.Contains(knownQueueName))
                {
                    // Only add queue name if it hasn't been added already. In
                    // this way, a given match will maintain its earliest
                    // position unless fully removed
                    matchedQueueIdentifiers.Add(knownQueueName);
                }
            }
        }
        return matchedQueueIdentifiers;
    }
}
