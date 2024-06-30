using Reqless.Client;
using Reqless.Tests.TestHelpers;
using StackExchange.Redis;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.UntrackJobAsync"/>.
/// </summary>
public class UntrackJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.UntrackJobAsync"/> should throw if jid is
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UntrackJobAsync(jid: null!)
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'jid')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UntrackJobAsync"/> should throw if jid is
    /// empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.UntrackJobAsync(jid: emptyString)
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.UntrackJobAsync"/> should throw if the server
    /// returns a null result.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UntrackJobAsync(ExampleJid),
                expectedArguments: ["job.untrack", 0, ExampleJid],
                returnValue: RedisValue.Null
            )
        );
        Assert.Equal(
            "Server returned unexpected null result.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UntrackJobAsync"/> calls the executor with
    /// the expected arguments, returning true when the server returns 1.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArgumentsReturningTrue()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                var untrackedSuccessfully = await subject.UntrackJobAsync(ExampleJid);
                Assert.True(untrackedSuccessfully);
            },
            expectedArguments: ["job.untrack", 0, ExampleJid],
            returnValue: 1
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UntrackJobAsync"/> calls the executor with
    /// the expected arguments, returning false when the server returns 0.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArgumentsReturningFalse()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                var untrackedSuccessfully = await subject.UntrackJobAsync(ExampleJid);
                Assert.False(untrackedSuccessfully);
            },
            expectedArguments: ["job.untrack", 0, ExampleJid],
            returnValue: 0
        );
    }
}