using Reqless.Client;
using Reqless.Tests.TestHelpers;
using StackExchange.Redis;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetJobsByStateAsync"/>.
/// </summary>
public class GetJobsByStateAsyncTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if state is
    /// null.
    /// </summary>
    [Fact]
    public async void ThrowsIfStateIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsByStateAsync(
                    queueName: ExampleQueueName,
                    state: null!
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'state')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if state is
    /// empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfStateIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.GetJobsByStateAsync(
                        queueName: ExampleQueueName,
                        state: emptyString
                    )
                )
            );
            Assert.Equal(
                "The value cannot be an empty string or composed entirely of whitespace. (Parameter 'state')",
                exception.Message
            );
        }
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if queue
    /// name is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsNull()
    {
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsByStateAsync(
                    queueName: null!,
                    state: "running"
                )
            )
        );
        Assert.Equal(
            "Value cannot be null. (Parameter 'queueName')",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if queue
    /// name is empty or whitespace.
    /// </summary>
    [Fact]
    public async void ThrowsIfQueueNameIsEmptyOrWhitespace()
    {
        foreach (var emptyString in TestConstants.EmptyStrings)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.GetJobsByStateAsync(
                        queueName: emptyString,
                        state: "waiting"
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
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if result
    /// is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfResultIsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsByStateAsync(
                    queueName: ExampleQueueName,
                    state: "running"
                ),
                expectedArguments: [
                    "queue.jobsByState",
                    0,
                    "running",
                    ExampleQueueName,
                    0,
                    25,
                ],
                returnValue: null
            )
        );
        Assert.Equal(
            "Server returned unexpected null result.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if any jid
    /// is null.
    /// </summary>
    [Fact]
    public async void ThrowsIfAnyJidIsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsByStateAsync(
                    queueName: ExampleQueueName,
                    state: "running"
                ),
                expectedArguments: [
                    "queue.jobsByState",
                    0,
                    "running",
                    ExampleQueueName,
                    0,
                    25,
                ],
                returnValues: [RedisValue.Null]
            )
        );
        Assert.Equal(
            "Server returned unexpected null jid.",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should return an empty
    /// list when there are no jobs in the given state.
    /// </summary>
    [Fact]
    public async void ReturnsEmptyListWhenNoSuchJobs()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                List<string> jobs = await subject.GetJobsByStateAsync(
                    limit: 25,
                    offset: 0,
                    queueName: ExampleQueueName,
                    state: "running"
                );
                Assert.Empty(jobs);
            },
            expectedArguments: [
                "queue.jobsByState",
                0,
                "running",
                ExampleQueueName,
                0,
                25,
            ],
            returnValues: []
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should return jids when
    /// there are jobs in the given state.
    /// </summary>
    [Fact]
    public async void ReturnsJidsWhenThereAreJobsInTheGivenState()
    {
        await WithClientWithExecutorMockForExpectedArguments(
            static async subject =>
            {
                List<string> jids = await subject.GetJobsByStateAsync(
                    limit: 25,
                    offset: 0,
                    queueName: ExampleQueueName,
                    state: "running"
                );
                var expectedJids = new string[] { ExampleJid, ExampleJidOther };
                Assert.Equal(expectedJids.Length, jids.Count);
                Assert.Contains(jids[0], expectedJids);
                Assert.Contains(jids[1], expectedJids);
            },
            expectedArguments: [
                "queue.jobsByState",
                0,
                "running",
                ExampleQueueName,
                0,
                25,
            ],
            returnValues: [ExampleJid, ExampleJidOther]
        );
    }
}
