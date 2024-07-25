using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.TimeoutJobAsync"/>.
/// </summary>
public class TimeoutJobTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobAsync"/> should call the executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsTheExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.TimeoutJobAsync(ExampleJid),
            expectedArguments: ["job.timeout", 0, ExampleJid]
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobAsync"/> should throw if the given
    /// jid is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.TimeoutJobAsync(null!),
                expectedArguments: ["job.timeout", 0, ExampleJid]
            )
        );
        Assert.Equal("Value cannot be null. (Parameter 'jid')", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobAsync"/> should throw if the given
    /// jid is empty or only whitespcae.
    /// </summary>
    [Fact]
    public async void ThrowsIfJidIsEmptyOrOnlyWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.TimeoutJobAsync(emptyString),
                    expectedArguments: ["job.timeout", 0, ExampleJid]
                )
            );
            Assert.Equal("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'jid')", exception.Message);
        }
    }
}