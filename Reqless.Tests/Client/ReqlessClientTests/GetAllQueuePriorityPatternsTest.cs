using System.Text.Json;
using Reqless.Client;
using Reqless.Models;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetAllQueuePriorityPatternsAsync"/>.
/// </summary>
public class GetAllQueuePriorityPatternsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueuePriorityPatternsAsync"/> should
    /// throw if server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueuePriorityPatternsAsync(),
                expectedArguments: ["queuePriorityPatterns.getAll", 0],
                returnValue: null
            )
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueuePriorityPatternsAsync"/> should
    /// throw if server returns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsInvalidJson()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueuePriorityPatternsAsync(),
                expectedArguments: ["queuePriorityPatterns.getAll", 0],
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize all queue priority patterns JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueuePriorityPatternsAsync"/> should
    /// throw if any of the priority patterns is null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfAnyPriorityPatternIsNull()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueuePriorityPatternsAsync(),
                expectedArguments: ["queuePriorityPatterns.getAll", 0],
                returnValue: """["null"]"""
            )
        );
        Assert.Equal(
            "Failed to deserialize queue priority pattern JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueuePriorityPatternsAsync"/> should
    /// call the executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        var expectedResult = new List<QueuePriorityPattern>() {
            new(pattern: ["other"], fairly: true),
            new(pattern: ["pattern"], fairly: false),
        };
        List<string> serializedPriorities = [];
        foreach (var priority in expectedResult)
        {
            serializedPriorities.Add(JsonSerializer.Serialize(priority));
        }
        var returnValue = JsonSerializer.Serialize(serializedPriorities);
        List<QueuePriorityPattern> queuePriorityPatterns = (
            await WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueuePriorityPatternsAsync(),
                expectedArguments: ["queuePriorityPatterns.getAll", 0],
                returnValue: returnValue
            )
        );
        Assert.Equivalent(expectedResult, queuePriorityPatterns);
    }
}