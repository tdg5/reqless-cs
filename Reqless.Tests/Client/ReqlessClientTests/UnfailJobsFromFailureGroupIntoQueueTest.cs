using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see
/// cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>.
/// </summary>
public class UnfailJobsFromFailureGroupIntoQueueTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if queue name is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                    groupName: ExampleGroupName, queueName: invalidQueueName!)),
            "queueName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if group name is null, empty, or only whitespace.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfGroupNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenArgumentIsNullOrEmptyOrWhitespaceAsync(
            (invalidGroupName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                    groupName: invalidGroupName!,
                    queueName: ExampleQueueName)),
            "groupName");
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if count is not a positive number.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfCountIsNotAPositiveNumber()
    {
        await Scenario.ThrowsWhenArgumentIsNotPositiveAsync(
            (invalidCount) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                    count: invalidCount,
                    groupName: ExampleGroupName,
                    queueName: ExampleQueueName)),
            "count");
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if server returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        var count = 1;
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                    count: count,
                    groupName: ExampleGroupName,
                    queueName: ExampleQueueName),
                expectedArguments: ["queue.unfail", 0, ExampleQueueName, ExampleGroupName, count],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should call the executor with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        var count = 10;
        var unfailedCount = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                count: count, groupName: ExampleGroupName, queueName: ExampleQueueName),
            expectedArguments: [
                "queue.unfail",
                0,
                ExampleQueueName,
                ExampleGroupName,
                count
            ],
            returnValue: count);
        Assert.Equal(count, unfailedCount);
    }
}
