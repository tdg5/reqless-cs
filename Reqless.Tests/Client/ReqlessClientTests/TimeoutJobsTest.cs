using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.TimeoutJobsAsync"/>.
/// </summary>
public class TimeoutJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobsAsync"/> should call the executor
    /// with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsTheExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.TimeoutJobsAsync(ExampleJid, ExampleJidOther),
            expectedArguments: ["job.timeout", 0, ExampleJid, ExampleJidOther]);
    }

    /// <summary>
    /// <see cref="ReqlessClient.TimeoutJobsAsync"/> should throw if any
    /// jid is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfAnyJidIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.TimeoutJobsAsync(invalidJid!, ExampleJidOther)),
            "jids");
    }
}
