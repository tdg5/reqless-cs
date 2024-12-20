using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.DeleteThrottleAsync"/>.
/// </summary>
public class DeleteThrottleTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.DeleteThrottleAsync"/> should throw if throttle
    /// name is null, or empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfThrottleNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidThrottleName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.DeleteThrottleAsync(invalidThrottleName!)),
            "throttleName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.DeleteThrottleAsync"/> should invoke the
    /// executor with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecuteAsyncWithExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.DeleteThrottleAsync(ExampleThrottleName),
            expectedArguments: ["throttle.delete", 0, ExampleThrottleName]);
    }
}
