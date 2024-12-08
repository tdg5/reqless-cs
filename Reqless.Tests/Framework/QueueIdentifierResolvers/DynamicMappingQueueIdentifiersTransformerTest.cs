using Moq;
using Reqless.Client;
using Reqless.Framework.QueueIdentifierResolvers;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Framework.QueueIdentifierResolvers;

/// <summary>
/// Unit tests for the <see cref="DynamicMappingQueueIdentifiersTransformer"/>
/// class.
/// </summary>
public class DynamicMappingQueueIdentifiersTransformerTest
{
    private readonly List<string> exampleQueueNames = [];

    /// <summary>
    /// Initializes a new instance of the <see
    /// cref="DynamicMappingQueueIdentifiersTransformerTest"/> class. This
    /// constructor initializes the <see cref="exampleQueueNames"/> list
    /// with example queue identifiers patterned like:
    /// [a1a, a2a, a3a, ...  g1g, g2g, g3g]
    /// Many queue identifiers are used to reduce the risk of shuffling a list
    /// into the same order.
    /// </summary>
    public DynamicMappingQueueIdentifiersTransformerTest()
    {
        foreach (var asciiOffset in Enumerable.Range(0, 7))
        {
            foreach (var index in Enumerable.Range(1, 3))
            {
                exampleQueueNames.Add(
                    $"{(char)(97 + asciiOffset)}{index}{(char)(97 + asciiOffset)}");
            }
        }
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer"/> constructor
    /// should throw when the Reqless client argument is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenReqlessClientIsNull()
    {
        Scenario.ThrowsWhenArgumentIsNull(
            () => new DynamicMappingQueueIdentifiersTransformer(null!),
            "reqlessClient");
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer"/> should
    /// throw when cache ttl milliseconds is negative.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsWhenCacheTtlMillisecondsIsNegative()
    {
        Scenario.ThrowsWhenArgumentIsNegative(
            (invalidTtl) => new DynamicMappingQueueIdentifiersTransformer(
                new Mock<IReqlessClient>().Object, invalidTtl),
            "cacheTtlMilliseconds");
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/>
    /// should cache queue identifier patterns for the configured duration.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_CachesQueueIdentifierPatterns()
    {
        var clientMock = new Mock<IReqlessClient>();
        clientMock.Setup(mock => mock.GetAllQueueIdentifierPatternsAsync())
            .Returns(Task.FromResult(new Dictionary<string, List<string>>()))
            .Verifiable(Times.Exactly(1));
        clientMock.Setup(mock => mock.GetAllQueueNamesAsync())
            .Returns(Task.FromResult(new List<string>()));
        IReqlessClient client = clientMock.Object;
        var subject = new DynamicMappingQueueIdentifiersTransformer(client, 60000);
        var queueNames = await subject.TransformAsync([]);
        queueNames = await subject.TransformAsync([]);
        clientMock.Verify();
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/>
    /// should fetch queue identifier patterns again after the cache expires.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_FetchesQueueIdentifierPatternsWhenCacheHasExpired()
    {
        var clientMock = new Mock<IReqlessClient>();
        clientMock.Setup(mock => mock.GetAllQueueIdentifierPatternsAsync())
            .Returns(Task.FromResult(new Dictionary<string, List<string>>()))
            .Verifiable(Times.Exactly(2));
        clientMock.Setup(mock => mock.GetAllQueueNamesAsync())
            .Returns(Task.FromResult(new List<string>()));
        IReqlessClient client = clientMock.Object;
        // TTL of 0 to force cache to expire immediately.
        var subject = new DynamicMappingQueueIdentifiersTransformer(client, 0);
        var queueNames = await subject.TransformAsync([]);
        queueNames = await subject.TransformAsync([]);
        clientMock.Verify();
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/> should
    /// be able to fetch queue identifier patterns following an exception that prevented queue
    /// identifier patterns from being fetched.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_CanFetchQueueIdentifierPatternsFollowingAnException()
    {
        var clientMock = new Mock<IReqlessClient>();
        clientMock.SetupSequence(mock => mock.GetAllQueueIdentifierPatternsAsync())
            .Throws(new Exception())
            .Returns(Task.FromResult(new Dictionary<string, List<string>>()));
        clientMock.Setup(mock => mock.GetAllQueueNamesAsync())
            .Returns(Task.FromResult(new List<string>()));
        IReqlessClient client = clientMock.Object;
        var subject = new DynamicMappingQueueIdentifiersTransformer(client);
        // This call fails.
        await Assert.ThrowsAsync<Exception>(() => subject.TransformAsync([]));
        // This call succeeds.
        await subject.TransformAsync([]);
        // This call should hit the cache.
        await subject.TransformAsync([]);
        clientMock.Verify();
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/> should
    /// prevent multiple cache refreshes from occurring simultaneously.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_OnlyAllowsOneCacheRefreshAtATime()
    {
        TaskCompletionSource<Dictionary<string, List<string>>> completionSource = new();
        var clientMock = new Mock<IReqlessClient>();
        clientMock.Setup(mock => mock.GetAllQueueIdentifierPatternsAsync())
            .Returns(completionSource.Task)
            .Verifiable(Times.Exactly(1));
        clientMock.Setup(mock => mock.GetAllQueueNamesAsync())
            .Returns(Task.FromResult(new List<string>()));
        IReqlessClient client = clientMock.Object;
        var subject = new DynamicMappingQueueIdentifiersTransformer(client);
        var queueNamesTask = subject.TransformAsync([]);
        var otherQueueNamesTask = subject.TransformAsync([]);

        // Make sure both tasks are blocked waiting for the queue identifier patterns.
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
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/>
    /// should only return queue names that match a whole pattern.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_MatchesWholePatternsOnly()
    {
        await WithSubject(
            action: async (subject) =>
            {
                var result = await subject.TransformAsync(
                    ["g3g", "2*", "*2", "*", "!a", "!2*", "!*2", "!a2ax", "!a1a"]);
                Assert.Equal(
                    [.. exampleQueueNames[^1..], .. exampleQueueNames[1..^1]], result);
            },
            queueIdentifierPatterns: [],
            knownQueueNames: exampleQueueNames);
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/>
    /// should match expected queues using inclusive wildcard patterns.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_MatchesInclusiveWildcardPatterns()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                [
                    .. exampleQueueNames.Where(_ => _.StartsWith('a')),
                    .. exampleQueueNames.Where(
                        _ => _.EndsWith('b') && !_.StartsWith('a')),
                    .. exampleQueueNames.Where(
                        _ => _.Contains('2') && !_.StartsWith('a') && !_.EndsWith('b')),
                ],
                await subject.TransformAsync(["a*", "*b", "*2*"])),
            queueIdentifierPatterns: [],
            knownQueueNames: exampleQueueNames);
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/>
    /// should match expected queues using exclusive wildcard patterns.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_MatchesExclusiveWildcardPatterns()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                exampleQueueNames.Where(
                    _ => !_.StartsWith('a') && !_.EndsWith('b') && !_.Contains('2')),
                await subject.TransformAsync(["*", "!a*", "!*b", "!*2*"])),
            queueIdentifierPatterns: [],
            knownQueueNames: exampleQueueNames);
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/>
    /// should match expected queues using dynamic inclusive wildcard patterns.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_MatchesDynamicInclusiveWildcardPatterns()
    {
        await WithSubject(
            action: async (subject) => Assert.Equal(
                [
                    .. exampleQueueNames.Where(_ => _.StartsWith('a')),
                    .. exampleQueueNames.Where(
                        _ => _.EndsWith('b') && !_.StartsWith('a') && !_.Contains('g')),
                    .. exampleQueueNames.Where(
                        _ => _.Contains('2') && !_.StartsWith('a')
                        && !_.EndsWith('b') && !_.Contains('g')),
                    exampleQueueNames[^1],
                ],
                await subject.TransformAsync(["@bar", "@baz", "@foo", "@fizz", "@buzz"])),
            queueIdentifierPatterns: new Dictionary<string, List<string>>
            {
                ["bar"] = ["a*"],
                ["baz"] = ["*b"],
                ["buzz"] = ["g3g"],
                ["fizz"] = ["!g*"],
                ["foo"] = ["*2*"],
            },
            knownQueueNames: exampleQueueNames);
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/>
    /// should match expected queues using dynamic exclusive wildcard patterns.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_MatchesDynamicExclusiveWildcardPatterns()
    {
        await WithSubject(
            action: async (subject) =>
            {
                var result = await subject.TransformAsync(
                    ["!@fizz", "*", "!@bar", "!@baz", "!@foo", "!@buzz", "!@bash"]);
                Assert.Equal(
                    [
                        .. exampleQueueNames.Where(
                            _ => _.StartsWith('g') && !_.Contains('2') && _ != "g3g"),
                        .. exampleQueueNames.Where(
                        _ => !_.StartsWith('a') && !_.StartsWith('g')
                            && !_.EndsWith('b') && !_.Contains('2')),
                        exampleQueueNames[1],
                    ],
                    result);
            },
            queueIdentifierPatterns: new Dictionary<string, List<string>>
            {
                ["bar"] = ["a*"],
                ["bash"] = ["!a2a"],
                ["baz"] = ["*b"],
                ["buzz"] = ["g3g"],
                ["fizz"] = ["!g*"],
                ["foo"] = ["*2*"],
            },
            knownQueueNames: exampleQueueNames);
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/>
    /// should always include static queue identifiers even if they don't match
    /// a known queue.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_ReturnsStaticIdentifiersNotMatchingKnownQueues()
    {
        List<string> expectedResult = ["bar", "baz", "foo"];
        await WithSubject(
            action: async (subject) => Assert.Equal(
                expectedResult,
                await subject.TransformAsync(expectedResult)),
            queueIdentifierPatterns: [],
            knownQueueNames: exampleQueueNames);
    }

    /// <summary>
    /// <see cref="DynamicMappingQueueIdentifiersTransformer.TransformAsync"/>
    /// should not return duplicate queue names.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task TransformAsync_DoesNotReturnDuplicates()
    {
        List<string> expectedResult = ["bar", "baz", "foo", "bar", "baz", "foo"];
        await WithSubject(
            action: async (subject) => Assert.Equal(
                expectedResult[..3],
                await subject.TransformAsync(expectedResult)),
            queueIdentifierPatterns: [],
            knownQueueNames: exampleQueueNames);
    }

    private static async Task WithSubject(
        Func<DynamicMappingQueueIdentifiersTransformer, Task> action,
        Dictionary<string, List<string>> queueIdentifierPatterns,
        List<string> knownQueueNames)
    {
        var clientMock = new Mock<IReqlessClient>();
        clientMock.Setup(mock => mock.GetAllQueueIdentifierPatternsAsync())
            .Returns(Task.FromResult(queueIdentifierPatterns));
        clientMock.Setup(mock => mock.GetAllQueueNamesAsync())
            .Returns(Task.FromResult(knownQueueNames));
        IReqlessClient client = clientMock.Object;
        await action(new DynamicMappingQueueIdentifiersTransformer(client));
        clientMock.Verify();
    }
}
