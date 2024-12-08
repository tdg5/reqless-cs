using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.SetQueueThrottleAsync"/>.
/// </summary>
public class SetQueueThrottleTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.SetQueueThrottleAsync"/> should throw if queue
    /// name is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetQueueThrottleAsync(
                    maximum: 21, queueName: invalidQueueName!)),
            "queueName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetQueueThrottleAsync"/> should throw if
    /// maximum is negative.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfMaximumIsLessThanZero()
    {
        await Scenario.ThrowsWhenArgumentIsNegativeAsync(
            (invalidMaximum) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetQueueThrottleAsync(
                    maximum: invalidMaximum, queueName: ExampleQueueName)),
            "maximum");
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetQueueThrottleAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        var newMaximum = 42;
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetQueueThrottleAsync(
                maximum: newMaximum, queueName: ExampleQueueName),
            expectedArguments: [
                "queue.throttle.set",
                0,
                ExampleQueueName,
                newMaximum,
            ]);
    }
}
