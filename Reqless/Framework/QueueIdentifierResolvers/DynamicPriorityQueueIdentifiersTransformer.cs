using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Common.Validation;
using System.Text.RegularExpressions;

namespace Reqless.Framework.QueueIdentifierResolvers;

/// <summary>
/// Implementation of <see cref="IQueueIdentifiersTransformer"/> that
/// transforms queue identifiers by sorting the identifiers based on dynamic
/// queue priority patterns.
/// </summary>
public class DynamicPriorityQueueIdentifiersTransformer : IQueueIdentifiersTransformer
{
    /// <summary>
    /// The <see cref="IClient"/> instance used to fetch queue priority
    /// patterns.
    /// </summary>
    protected IClient ReqlessClient { get; }

    /// <summary>
    /// The time-to-live in milliseconds for the queue priority patterns cache.
    /// </summary>
    protected int CacheTtlMilliseconds { get; }

    private long _cacheExpiresAt = 0;

    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    private List<QueuePriorityPattern> _queuePriorityPatterns = [];

    /// <summary>
    /// Create an instance of <see
    /// cref="DynamicPriorityQueueIdentifiersTransformer"/>.
    /// </summary>
    /// <param name="reqlessClient">The <see cref="IClient"/> instance used to
    /// fetch queue priority patterns.</param>
    /// <param name="cacheTtlMilliseconds">The time-to-live in milliseconds for
    /// the queue priority patterns cache.</param>
    public DynamicPriorityQueueIdentifiersTransformer(
        IClient reqlessClient,
        int cacheTtlMilliseconds = 60000
    )
    {
        ArgumentNullException.ThrowIfNull(reqlessClient, nameof(reqlessClient));
        ArgumentValidation.ThrowIfNegative(cacheTtlMilliseconds, nameof(cacheTtlMilliseconds));

        ReqlessClient = reqlessClient;
        CacheTtlMilliseconds = cacheTtlMilliseconds;
    }

    /// <summary>
    /// Transform the given queue identifiers by reordering the queue
    /// identifiers based on the queue priority patterns returned by <see
    /// cref="IClient.GetAllQueuePriorityPatternsAsync"/>.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers that should be
    /// mapped to their respective concrete sorted queue names.</param>
    /// <returns></returns>
    public async Task<List<string>> TransformAsync(List<string> queueIdentifiers)
    {
        var queuePriorityPatterns = await GetQueuePriorityPatternsAsync();
        return SortQueueIdentifiers(queueIdentifiers, queuePriorityPatterns);
    }

    /// <summary>
    /// Get the queue priority patterns either from the in-memory cache or by
    /// fetching the patterns from the Reqless backend.
    /// </summary>
    /// <returns>The list of <see cref="QueuePriorityPattern"/>
    /// instances.</returns>
    protected async Task<List<QueuePriorityPattern>> GetQueuePriorityPatternsAsync()
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
                    _queuePriorityPatterns = await ReqlessClient.GetAllQueuePriorityPatternsAsync();
                    _cacheExpiresAt = now + CacheTtlMilliseconds;
                }
            }
            finally
            {
                _cacheLock.Release();
            }
        }
        return _queuePriorityPatterns;
    }

    /// <summary>
    /// Sort the given queue identifiers based on the queue priority patterns.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers to sort.</param>
    /// <param name="queuePriorityPatterns">The queue priority patterns defining
    /// the priority buckets.</param>
    /// <returns>The sorted queue identifiers.</returns>
    protected static List<string> SortQueueIdentifiers(
        List<string> queueIdentifiers,
        List<QueuePriorityPattern> queuePriorityPatterns
    )
    {
        List<List<string>> prioritizedQueueGroups = [];
        List<string> _queueIdentifiers = [.. queueIdentifiers];

        Random random = new();
        int defaultIndex = -1;
        bool defaultShouldDistributeFairly = false;

        foreach (var queuePriorityPattern in queuePriorityPatterns)
        {
            if (
                queuePriorityPattern.Pattern.Count == 1
                && queuePriorityPattern.Pattern[0] == "default"
            )
            {
                defaultIndex = prioritizedQueueGroups.Count;
                defaultShouldDistributeFairly = queuePriorityPattern.Fairly;
                continue;
            }

            List<string> priorityGroupIdentifiers = [];
            foreach (string pattern in queuePriorityPattern.Pattern)
            {
                bool negated = pattern.StartsWith('!');
                string _pattern = (negated ? pattern[1..] : pattern).Replace("*", ".*");
                string anchoredPattern = $"^{_pattern}$";

                if (negated)
                {
                    List<string> patternsToRemove = [];
                    foreach (var identifier in priorityGroupIdentifiers)
                    {
                        if (Regex.IsMatch(identifier, anchoredPattern))
                        {
                            patternsToRemove.Add(identifier);
                        }
                    }
                    // Remove patterns as a separate step to avoid modifying the
                    // collection while iterating over it.
                    foreach (var patternToRemove in patternsToRemove)
                    {
                        priorityGroupIdentifiers.Remove(patternToRemove);
                    }
                }
                else
                {
                    foreach (var identifier in _queueIdentifiers)
                    {
                        if (
                            Regex.IsMatch(identifier, anchoredPattern)
                            && !priorityGroupIdentifiers.Contains(identifier)
                        )
                        {
                            priorityGroupIdentifiers.Add(identifier);
                        }
                    }
                }
            }

            foreach (var identifier in priorityGroupIdentifiers)
            {
                _queueIdentifiers.Remove(identifier);
            }

            List<string> _priorityGroupIdentifiers = queuePriorityPattern.Fairly
                ? [.. priorityGroupIdentifiers.OrderBy(_ => random.Next())]
                : priorityGroupIdentifiers;
            prioritizedQueueGroups.Add(_priorityGroupIdentifiers);
        }

        List<string> defaultBucketIdentifiers = defaultShouldDistributeFairly
            ? [.. _queueIdentifiers.OrderBy(_ => random.Next())]
            : _queueIdentifiers;

        int _defaultIndex = defaultIndex == -1
            ? prioritizedQueueGroups.Count : defaultIndex;
        prioritizedQueueGroups.Insert(_defaultIndex, defaultBucketIdentifiers);

        return prioritizedQueueGroups.SelectMany(_ => _).ToList();
    }
}
