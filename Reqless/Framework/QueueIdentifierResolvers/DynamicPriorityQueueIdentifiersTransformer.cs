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
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    private long _cacheExpiresAt = 0;

    private List<QueuePriorityPattern> _queuePriorityPatterns = [];

    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="DynamicPriorityQueueIdentifiersTransformer"/> class.
    /// </summary>
    /// <param name="reqlessClient">The <see cref="IReqlessClient"/> instance used to
    /// fetch queue priority patterns.</param>
    /// <param name="cacheTtlMilliseconds">The time-to-live in milliseconds for
    /// the queue priority patterns cache.</param>
    public DynamicPriorityQueueIdentifiersTransformer(
        IReqlessClient reqlessClient, int cacheTtlMilliseconds = 60000)
    {
        ArgumentNullException.ThrowIfNull(reqlessClient, nameof(reqlessClient));
        ArgumentValidation.ThrowIfNegative(cacheTtlMilliseconds, nameof(cacheTtlMilliseconds));

        ReqlessClient = reqlessClient;
        CacheTtlMilliseconds = cacheTtlMilliseconds;
    }

    /// <summary>
    /// Gets the <see cref="IReqlessClient"/> instance used to fetch queue priority
    /// patterns.
    /// </summary>
    protected IReqlessClient ReqlessClient { get; }

    /// <summary>
    /// Gets the time-to-live in milliseconds for the queue priority patterns cache.
    /// </summary>
    protected int CacheTtlMilliseconds { get; }

    /// <summary>
    /// Transform the given queue identifiers by reordering the queue
    /// identifiers based on the queue priority patterns returned by <see
    /// cref="IReqlessClient.GetAllQueuePriorityPatternsAsync"/>.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers that should be
    /// mapped to their respective concrete sorted queue names.</param>
    /// <returns>The transformed list of queue identifiers.</returns>
    public async Task<List<string>> TransformAsync(List<string> queueIdentifiers)
    {
        var queuePriorityPatterns = await GetQueuePriorityPatternsAsync();
        return SortQueueIdentifiers(queueIdentifiers, queuePriorityPatterns);
    }

    /// <summary>
    /// Sort the given queue identifiers based on the queue priority patterns.
    /// </summary>
    /// <param name="queueIdentifiers">The queue identifiers to sort.</param>
    /// <param name="queuePriorityPatterns">The queue priority patterns defining
    /// the priority buckets.</param>
    /// <returns>The sorted queue identifiers.</returns>
    protected static List<string> SortQueueIdentifiers(
        List<string> queueIdentifiers, List<QueuePriorityPattern> queuePriorityPatterns)
    {
        List<List<string>> prioritizedQueueGroups = [];
        List<string> queueIdentifiersCopy = [.. queueIdentifiers];

        Random random = new();
        int defaultIndex = -1;
        bool defaultShouldDistributeFairly = false;

        foreach (var queuePriorityPattern in queuePriorityPatterns)
        {
            if (
                queuePriorityPattern.Pattern.Count == 1
                && queuePriorityPattern.Pattern[0] == "default")
            {
                defaultIndex = prioritizedQueueGroups.Count;
                defaultShouldDistributeFairly = queuePriorityPattern.Fairly;
                continue;
            }

            List<string> priorityGroupIdentifiers = [];
            foreach (string pattern in queuePriorityPattern.Pattern)
            {
                bool negated = pattern.StartsWith('!');
                string sanitizedPattern = (negated ? pattern[1..] : pattern).Replace("*", ".*");
                string anchoredPattern = $"^{sanitizedPattern}$";

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
                    foreach (var identifier in queueIdentifiersCopy)
                    {
                        if (
                            Regex.IsMatch(identifier, anchoredPattern)
                            && !priorityGroupIdentifiers.Contains(identifier))
                        {
                            priorityGroupIdentifiers.Add(identifier);
                        }
                    }
                }
            }

            foreach (var identifier in priorityGroupIdentifiers)
            {
                queueIdentifiersCopy.Remove(identifier);
            }

            List<string> sortedPriorityGroupIdentifiers = queuePriorityPattern.Fairly
                ? [.. priorityGroupIdentifiers.OrderBy(_ => random.Next())]
                : priorityGroupIdentifiers;
            prioritizedQueueGroups.Add(sortedPriorityGroupIdentifiers);
        }

        List<string> defaultBucketIdentifiers = defaultShouldDistributeFairly
            ? [.. queueIdentifiersCopy.OrderBy(_ => random.Next())]
            : queueIdentifiersCopy;

        int defaultIndexOrLast = defaultIndex == -1
            ? prioritizedQueueGroups.Count : defaultIndex;
        prioritizedQueueGroups.Insert(defaultIndexOrLast, defaultBucketIdentifiers);

        return prioritizedQueueGroups.SelectMany(_ => _).ToList();
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
                    _queuePriorityPatterns =
                        await ReqlessClient.GetAllQueuePriorityPatternsAsync();
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
}
