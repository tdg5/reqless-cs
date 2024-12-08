using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

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
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnpauseQueueAsync(queueName: invalidQueueName!)),
            "queueName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnpauseQueueAsync"/> should call the executor
    /// with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UnpauseQueueAsync(ExampleQueueName),
            expectedArguments: ["queue.unpause", 0, ExampleQueueName]);
    }
}
