using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.ReleaseThrottleForJobsAsync"/>.
/// </summary>
public class ReleaseThrottleForJobsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.ReleaseThrottleForJobsAsync"/> should call the
    /// executor with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsTheExecutorWithTheExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.ReleaseThrottleForJobsAsync(
                ExampleThrottleName, ExampleJid),
            expectedArguments: [
                "throttle.release",
                0,
                ExampleThrottleName,
                ExampleJid,
            ]);
    }

    /// <summary>
    /// <see cref="ReqlessClient.ReleaseThrottleForJobsAsync"/> throws if the
    /// given jids argument is null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJidsIsNull()
    {
        await Scenario.ThrowsWhenArgumentIsNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ReleaseThrottleForJobsAsync(
                    ExampleThrottleName, null!)),
            "jids");
    }

    /// <summary>
    /// <see cref="ReqlessClient.ReleaseThrottleForJobsAsync"/> throws if any of
    /// the given JIDs are null, empty, or whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfAnyOfTheJidsAreNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            (invalidJid) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ReleaseThrottleForJobsAsync(
                    ExampleThrottleName, ExampleJid, null!)),
            "jids");
    }
}
