using Reqless.Client;
using Reqless.Tests.TestHelpers;

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
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetQueueThrottleAsync(
                    queueName: invalidQueueName!,
                    maximum: 21
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetQueueThrottleAsync"/> should throw if
    /// maximum is negative.
    /// </summary>
    [Fact]
    public async Task ThrowsIfMaximumIsLessThanZero()
    {
        await Scenario.ThrowsWhenParameterIsNegativeAsync(
            (invalidMaximum) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.SetQueueThrottleAsync(
                    queueName: ExampleQueueName,
                    maximum: invalidMaximum
                )
            ),
            "maximum"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.SetQueueThrottleAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        var newMaximum = 42;
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.SetQueueThrottleAsync(
                queueName: ExampleQueueName,
                maximum: newMaximum
            ),
            expectedArguments: [
                "queue.throttle.set",
                0,
                ExampleQueueName,
                newMaximum,
            ]
        );
    }
}
