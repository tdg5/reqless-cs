using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.PauseQueueAsync"/>.
/// </summary>
public class PauseQueueTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.PauseQueueAsync"/> should throw if queue
    /// name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.PauseQueueAsync(queueName: invalidQueueName!)
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.PauseQueueAsync"/> should call the executor
    /// with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.PauseQueueAsync(ExampleQueueName),
            expectedArguments: ["queue.pause", 0, ExampleQueueName]
        );
    }
}
