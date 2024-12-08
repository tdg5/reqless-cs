using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.ForgetQueuesAsync"/>.
/// </summary>
public class ForgetQueuesTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.ForgetQueuesAsync"/> throws if the given
    /// collection is null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNamesIsNull()
    {
        await Scenario.ThrowsWhenArgumentIsNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ForgetQueuesAsync(null!)),
            "queueNames");
    }

    /// <summary>
    /// <see cref="ReqlessClient.ForgetQueuesAsync"/> throws if any of the given
    /// queue names are null, empty string, or strings composed entirely of
    /// whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfAnyQueueNameIsNullEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentElementIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.ForgetQueuesAsync(invalidQueueName!)),
            "queueNames");
    }

    /// <summary>
    /// <see cref="ReqlessClient.ForgetQueuesAsync"/> calls the executor with the
    /// expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var otherQueue = "other-queue";
        await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.ForgetQueuesAsync(ExampleQueueName, otherQueue),
            expectedArguments: ["queue.forget", 0, ExampleQueueName, otherQueue]);
    }
}
