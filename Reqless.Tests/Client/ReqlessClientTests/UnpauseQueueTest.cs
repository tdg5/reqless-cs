using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.UnpauseQueueAsync"/>.
/// </summary>
public class UnpauseQueueTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.UnpauseQueueAsync"/> should throw if queue
    /// name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnpauseQueueAsync(queueName: invalidQueueName!)
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnpauseQueueAsync"/> should call the executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UnpauseQueueAsync(ExampleQueueName),
            expectedArguments: ["queue.unpause", 0, ExampleQueueName]
        );
    }
}