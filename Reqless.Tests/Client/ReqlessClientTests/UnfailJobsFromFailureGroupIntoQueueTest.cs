using Reqless.Client;
using Reqless.Tests.TestHelpers;

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
    [Fact]
    public async void ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                    groupName: ExampleGroupName,
                    queueName: invalidQueueName!
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if group name is null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfGroupNameIsNullOrEmptyOrWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidGroupName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                    groupName: invalidGroupName!,
                    queueName: ExampleQueueName
                )
            ),
            "groupName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if count is not a positive number.
    /// </summary>
    [Fact]
    public async void ThrowsIfCountIsNotAPositiveNumber()
    {
        await Scenario.ThrowsWhenParameterIsNotPositiveAsync(
            (invalidCount) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                    count: invalidCount,
                    groupName: ExampleGroupName,
                    queueName: ExampleQueueName
                )
            ),
            "count"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if server returns null.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
    {
        var count = 1;
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                    count: count,
                    groupName: ExampleGroupName,
                    queueName: ExampleQueueName
                ),
                expectedArguments: ["queue.unfail", 0, ExampleQueueName, ExampleGroupName, count],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should call the executor with the expected arguments.
    /// </summary>
    [Fact]
    public async void CallsExecutorWithExpectedArguments()
    {
        var count = 10;
        var unfailedCount = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                count: count,
                groupName: ExampleGroupName,
                queueName: ExampleQueueName
            ),
            expectedArguments: [
                "queue.unfail",
                0,
                ExampleQueueName,
                ExampleGroupName,
                count
            ],
            returnValue: count
        );
        Assert.Equal(count, unfailedCount);
    }
}