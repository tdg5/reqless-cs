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
    /// should throw if queue name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                    groupName: ExampleGroupName,
                    queueName: null!
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'queueName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if queue name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                        groupName: ExampleGroupName,
                        queueName: emptyString
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'queueName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if group name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfGroupNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                    groupName: null!,
                    queueName: ExampleQueueName
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'groupName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if group name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfGroupNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                        groupName: emptyString,
                        queueName: ExampleQueueName
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'groupName')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.UnfailJobsFromFailureGroupIntoQueueAsync"/>
    /// should throw if count is less than 1.
    /// </summary>
    [Fact]
    public async void ThrowsIfCountIsLessThanOne()
    {
        foreach (var invalidCount in new int[] { -100, -10, -1, 0 })
        {
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.UnfailJobsFromFailureGroupIntoQueueAsync(
                        count: invalidCount,
                        groupName: ExampleGroupName,
                        queueName: ExampleQueueName
                    )
                )
            );
            Assert.Equal(
                "Value must be greater than zero. (Parameter 'count')",
                exception.Message
            );
        }
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