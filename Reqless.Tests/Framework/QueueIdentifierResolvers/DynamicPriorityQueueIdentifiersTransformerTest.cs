using Moq;
using Reqless.Client;
using Reqless.Client.Models;
using Reqless.Framework.QueueIdentifierResolvers;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Framework.QueueIdentifierResolvers;

/// <summary>
/// Unit tests for the <see cref="DynamicPriorityQueueIdentifiersTransformer"/>
/// class.
/// </summary>
public class DynamicPriorityQueueIdentifiersTransformerTest
{
    private static readonly QueuePriorityPattern DefaultQueuePriorityPattern =
        new(["default"], fairly: false);

    private readonly List<string> exampleQueueIdentifiers = [];

    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="DynamicPriorityQueueIdentifiersTransformerTest"/> class. This
    /// constructor initializes the <see cref="exampleQueueIdentifiers"/> list
    /// with example queue identifiers patterned like:
    /// [a1a, a2a, a3a, ...  g1g, g2g, g3g]
    /// Many queue identifiers are used to reduce the risk of shuffling a list
    /// into the same order.
    /// </summary>
    public DynamicPriorityQueueIdentifiersTransformerTest()
    {
        foreach (var asciiOffset in Enumerable.Range(0, 7))
        {
            foreach (var index in Enumerable.Range(1, 3))
            {
                exampleQueueIdentifiers.Add(
                    $"{(char)(97 + asciiOffset)}{index}{(char)(97 + asciiOffset)}");
            }
        }
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer"/> constructor
    /// should throw when the Reqless client argument is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenReqlessClientIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DynamicPriorityQueueIdentifiersTransformer(null!),
            "reqlessClient");
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer"/> should
    /// throw when cache ttl milliseconds is negative.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenCacheTtlMillisecondsIsNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (invalidTtl) => new DynamicPriorityQueueIdentifiersTransformer(
                new Mock<IReqlessClient>().Object,
                invalidTtl),
            "cacheTtlMilliseconds");
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should cache queue priority patterns for the configured duration.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_CachesQueuePriorityPatterns()
    {
        var clientMock = new Mock<IReqlessClient>();
        clientMock.Setup(mock => mock.GetAllQueuePriorityPatternsAsync())
            .Returns(Task.FromResult(new List<QueuePriorityPattern>()))
            .Verifiable(Times.Exactly(1));
        IReqlessClient client = clientMock.Object;
        var subject = new DynamicPriorityQueueIdentifiersTransformer(client, 60000);
        var queueNames = await subject.TransformAsync([]);
        queueNames = await subject.TransformAsync([]);
        clientMock.Verify();
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should fetch queue priority patterns again after the cache expires.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_FetchesQueuePriorityPatternsWhenCacheHasExpired()
    {
        var clientMock = new Mock<IReqlessClient>();
        clientMock.Setup(mock => mock.GetAllQueuePriorityPatternsAsync())
            .Returns(Task.FromResult(new List<QueuePriorityPattern>()))
            .Verifiable(Times.Exactly(2));
        IReqlessClient client = clientMock.Object;
        // TTL of 0 to force cache to expire immediately.
        var subject = new DynamicPriorityQueueIdentifiersTransformer(client, 0);
        var queueNames = await subject.TransformAsync([]);
        queueNames = await subject.TransformAsync([]);
        clientMock.Verify();
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/> should
    /// be able to fetch queue priority patterns following an exception that prevented queue
    /// priority patterns from being fetched.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_CanFetchQueuePriorityPatternsFollowingAnException()
    {
        var clientMock = new Mock<IReqlessClient>();
        clientMock.SetupSequence(mock => mock.GetAllQueuePriorityPatternsAsync())
            .Throws(new Exception())
            .Returns(Task.FromResult(new List<QueuePriorityPattern>()));
        IReqlessClient client = clientMock.Object;
        var subject = new DynamicPriorityQueueIdentifiersTransformer(client);
        // This call fails.
        await Assert.ThrowsAsync<Exception>(() => subject.TransformAsync([]));
        // This call succeeds.
        await subject.TransformAsync([]);
        // This call should hit the cache.
        await subject.TransformAsync([]);
        clientMock.Verify();
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/> should
    /// prevent multiple cache refreshes from occurring simultaneously.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_OnlyAllowsOneCacheRefreshAtATime()
    {
        TaskCompletionSource<List<QueuePriorityPattern>> completionSource = new();
        var clientMock = new Mock<IReqlessClient>();
        clientMock.Setup(mock => mock.GetAllQueuePriorityPatternsAsync())
            .Returns(completionSource.Task)
            .Verifiable(Times.Exactly(1));
        IReqlessClient client = clientMock.Object;
        var subject = new DynamicPriorityQueueIdentifiersTransformer(client);
        var queueNamesTask = subject.TransformAsync([]);
        var otherQueueNamesTask = subject.TransformAsync([]);

        // Make sure both tasks are blocked waiting for the queue priority patterns.
        foreach (var task in new[] { queueNamesTask, otherQueueNamesTask })
        {
            await Assert.ThrowsAsync<TimeoutException>(
                () => task.WaitAsync(TimeSpan.FromMilliseconds(100)));
            Assert.False(task.IsCompleted);
        }

        completionSource.SetResult([]);
        await Task.WhenAll(queueNamesTask, otherQueueNamesTask);
        clientMock.Verify();
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should not mutate sort order when no patterns are defined. This is akin
    /// to placing default queues last, unfairly.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransfromAsync_DoesNotMutateSortOrderWhenNoPatternsDefined()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                exampleQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: []);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should not mutate sort order when only the default pattern is defined.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransfromAsync_DoesNotMutateSortOrderWhenOnlyDefaultPattern()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                exampleQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [DefaultQueuePriorityPattern]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should place unmatched queues last, unfairly when no default pattern is
    /// defined.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_SortsDefaultsLastUnfairlyWhenNoDefaultPattern()
    {
        List<string> expectedQueueIdentifiers = [
            // Matched queues.
            .. exampleQueueIdentifiers[^3..],
            // Unmatched queues.
            .. exampleQueueIdentifiers[..^3],
        ];
        await WithSubject(
            action: async (subject) => Assert.Equal(
                expectedQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [new(["g*"], fairly: false)]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should place unmatched queues last, fairly when the default pattern is
    /// configured fairly.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_SortsDefaultsLastFairlyWhenDefaultPatternIsFair()
    {
        await WithSubject(
            action: async (subject) =>
            {
                var result = await subject.TransformAsync(exampleQueueIdentifiers);
                Assert.NotEqual(exampleQueueIdentifiers, result);
                result.Sort();
                Assert.Equal(exampleQueueIdentifiers, result);
            },
            queuePriorityPatterns: [new(["default"], fairly: true)]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should handle inclusive patterns with out wildcards unfairly.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_HandlesSimpleInclusivePatternsUnfairly()
    {
        List<string> expectedQueueIdentifiers = [
            .. exampleQueueIdentifiers[^3..],
            .. exampleQueueIdentifiers[3..^3],
            .. exampleQueueIdentifiers[..3],
        ];
        await WithSubject(
            action: async (subject) => Assert.Equal(
                expectedQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["g1g", "g2g", "g3g"], fairly: false),
                DefaultQueuePriorityPattern,
                new(["a1a", "a2a", "a3a"], fairly: false)
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should handle inclusive patterns with wildcards unfairly.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_HandlesWildcardInclusivePatternsUnfairly()
    {
        List<string> expectedQueueIdentifiers = [
            .. exampleQueueIdentifiers[^3..],
            .. exampleQueueIdentifiers[3..^3],
            .. exampleQueueIdentifiers[..3],
        ];
        await WithSubject(
            action: async (subject) => Assert.Equal(
                expectedQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["g*"], fairly: false),
                DefaultQueuePriorityPattern,
                new(["a*"], fairly: false)
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should handle inclusive patterns with wildcards fairly.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_HandlesWildcardInclusivePatternsFairly()
    {
        await WithSubject(
            action: async (subject) =>
            {
                var result = await subject.TransformAsync(exampleQueueIdentifiers);
                var expectedStartingSet = exampleQueueIdentifiers[^9..];
                var expectedEndingSet = exampleQueueIdentifiers[..9];
                var startingSet = result[..9];
                var endingSet = result[^9..];

                Assert.NotEqual(startingSet, expectedStartingSet);
                startingSet.Sort();
                Assert.Equal(startingSet, expectedStartingSet);

                Assert.NotEqual(endingSet, expectedEndingSet);
                endingSet.Sort();
                Assert.Equal(endingSet, expectedEndingSet);

                Assert.NotEqual(exampleQueueIdentifiers, result);
                result.Sort();
                Assert.Equal(exampleQueueIdentifiers, result);
            },
            queuePriorityPatterns: [
                new(["e*", "f*", "g*"], fairly: true),
                DefaultQueuePriorityPattern,
                new(["a*", "b*", "c*"], fairly: true)
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should handle inclusive patterns with double wildcards unfairly.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_HandlesDoubleWildcardInclusivePatternsUnfairly()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                [
                    .. exampleQueueIdentifiers.Where(_ => _.Contains('3')),
                    .. exampleQueueIdentifiers.Where(
                        _ => !_.Contains('1') && !_.Contains('3')),
                    .. exampleQueueIdentifiers.Where(_ => _.Contains('1')),
                ],
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["*3*"], fairly: false),
                DefaultQueuePriorityPattern,
                new(["*1*"], fairly: false)
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should match whole patterns only.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_MatchesWholePatternsOnly()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                exampleQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["g", "2*", "*2", "b2bx", "*", "!a", "!2*", "!*2"], fairly: false),
                DefaultQueuePriorityPattern,
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should handle simple exclusive patterns unfairly.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_HandlesSimpleExclusivePatternsUnfairly()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                [
                    .. exampleQueueIdentifiers[1..],
                    exampleQueueIdentifiers[0],
                ],
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["*", "!a1a"], fairly: false),
                DefaultQueuePriorityPattern,
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should handle wildcard exclusive patterns unfairly.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_HandlesWildcardExclusivePatternsUnfairly()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                [
                    .. exampleQueueIdentifiers[3..],
                    .. exampleQueueIdentifiers[..3],
                ],
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["*", "!a*"], fairly: false),
                DefaultQueuePriorityPattern,
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should handle exclusive patterns with double wildcards unfairly.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_HandlesDoubleWildcardExclusivePatternsUnfairly()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                [
                    .. exampleQueueIdentifiers.Where(
                        _ => !_.Contains('1') && !_.Contains('3')),
                    .. exampleQueueIdentifiers.Where(
                        _ => _.Contains('1') || _.Contains('3')),
                ],
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["*", "!*1*", "!*3*"], fairly: false),
                DefaultQueuePriorityPattern,
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should not be impacted by a standalone exclusive pattern.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_SortOrderNotImpactedByStandaloneExclusivePattern()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                exampleQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["!a1a"], fairly: false),
                DefaultQueuePriorityPattern,
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should work as expected when no queues fall in the default bucket.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_WorksWhenNoQueuesLeftForDefaultBucket()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                exampleQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["*"], fairly: false),
                DefaultQueuePriorityPattern,
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should evaluate patterns in the order they are defined, allowing later
    /// patterns to negate earlier patterns.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_EvaluatesPatternsInOrder()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                exampleQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["g*", "!g*"], fairly: false),
                DefaultQueuePriorityPattern,
            ]);
        // This one is kind of weird. Since starting with an exclusion pattern
        // doesn't filter anything out, we must first select everything, filter
        // something out, and then add that something back in. This changes sort
        // order in a way that may not be expected, but honors the notion of
        // evaluating patterns in order.
        await WithSubject(
            action: async (subject) => Assert.Equal(
                [
                    .. exampleQueueIdentifiers[3..],
                    .. exampleQueueIdentifiers[..3],
                ],
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["*", "!a*", "a*"], fairly: false),
                DefaultQueuePriorityPattern,
            ]);
    }

    /// <summary>
    /// <see cref="DynamicPriorityQueueIdentifiersTransformer.TransformAsync"/>
    /// should not repeat a queue identifier that is matched by multiple patterns.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_DoesNotRepeatQueueIdentifiers()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                exampleQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["a1a"], fairly: false),
                new(["a1a*"], fairly: false),
                new(["a1*"], fairly: false),
                new(["a*"], fairly: false),
                DefaultQueuePriorityPattern,
                new(["a*"], fairly: false),
                new(["a1*"], fairly: false),
                new(["a1a*"], fairly: false),
                new(["a1a"], fairly: false),
            ]);
        await WithSubject(
            action: async (subject) => Assert.Equal(
                exampleQueueIdentifiers,
                await subject.TransformAsync(exampleQueueIdentifiers)),
            queuePriorityPatterns: [
                new(["!a*", "a*"], fairly: false),
                DefaultQueuePriorityPattern,
            ]);
    }

    private static async Task WithSubject(
        List<QueuePriorityPattern> queuePriorityPatterns,
        Func<DynamicPriorityQueueIdentifiersTransformer, Task> action)
    {
        var clientMock = new Mock<IReqlessClient>();
        clientMock.Setup(mock => mock.GetAllQueuePriorityPatternsAsync())
            .Returns(Task.FromResult(queuePriorityPatterns));
        IReqlessClient client = clientMock.Object;
        await action(new DynamicPriorityQueueIdentifiersTransformer(client));
        clientMock.Verify();
    }
}
