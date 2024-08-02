using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetJobsByStateAsync"/>.
/// </summary>
public class GetJobsByStateAsyncTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if state is
    /// null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfStateIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidState) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsByStateAsync(
                    queueName: ExampleQueueName,
                    state: invalidState!
                )
            ),
            "state"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if queue
    /// name is null, empty, or whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfQueueNameIsNullOrEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenParameterIsNullOrEmptyOrWhitespaceAsync(
            (invalidQueueName) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetJobsByStateAsync(
                    queueName: invalidQueueName!,
                    state: "running"
                )
            ),
            "queueName"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if result
    /// is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfResultIsNull()
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
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if result
    /// JSON cannot be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfResultJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
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
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should throw if any jid
    /// is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfAnyJidIsNull()
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
                returnValue: "[null]"
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
    public async Task ReturnsEmptyListWhenNoSuchJobs()
    {
        List<string> jobs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetJobsByStateAsync(
                limit: 25,
                offset: 0,
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
            returnValue: "[]"
        );
        Assert.Empty(jobs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetJobsByStateAsync"/> should return no jids
    /// when there are jobs in the given state.
    /// </summary>
    [Fact]
    public async Task ReturnsNoJidsWhenThereAreJobsInTheGivenState()
    {
        List<string> jids = await WithClientWithExecutorMockForExpectedArguments(
             subject => subject.GetJobsByStateAsync(
                     limit: 25,
                     offset: 0,
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
             returnValue: JsonSerializer.Serialize(
                 new string[] { ExampleJid, ExampleJidOther }
             )
         );
        var expectedJids = new string[] { ExampleJid, ExampleJidOther };
        Assert.Equal(expectedJids.Length, jids.Count);
        Assert.Contains(jids[0], expectedJids);
        Assert.Contains(jids[1], expectedJids);
    }
}
