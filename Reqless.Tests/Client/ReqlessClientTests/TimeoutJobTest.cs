using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsTheExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.TimeoutJobAsync(ExampleJid),
            expectedArguments: ["job.timeout", 0, ExampleJid]);
    }

    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobAsync"/> should throw if the given
    /// jid is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.TimeoutJobAsync(invalidJid!)),
            "jid");
    }
}
