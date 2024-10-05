using Reqless.Client.Models;
using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetAllWorkerCountsAsync"/>.
/// </summary>
public class GetAllWorkerCountsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetAllWorkerCountsAsync"/> should throw if
    /// server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllWorkerCountsAsync(),
                expectedArguments: ["workers.counts", 0],
                returnValue: null
            )
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllWorkerCountsAsync"/> should throw if
    /// server retruns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllWorkerCountsAsync(),
                expectedArguments: ["workers.counts", 0],
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize all worker counts JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllWorkerCountsAsync"/> should return an
    /// empty array when the server returns an empty JSON object.
    /// </summary>
    [Fact]
    public async Task ReturnsEmptyArrayWhenServerReturnsJsonObject()
    {
        var allWorkerCounts = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetAllWorkerCountsAsync(),
            expectedArguments: ["workers.counts", 0],
            returnValue: "{}"
        );
        Assert.Empty(allWorkerCounts);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllWorkerCountsAsync"/> should return a
    /// valid result returned by the server.
    /// </summary>
    [Fact]
    public async Task ReturnsValidResultFromTheServer()
    {
        var expectedCounts = new WorkerCounts
        {
            Jobs = 0,
            Stalled = 0,
            WorkerName = ExampleWorkerName,
        };
        var countsJson = JsonSerializer.Serialize<WorkerCounts[]>([expectedCounts]);
        var allWorkerCounts = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetAllWorkerCountsAsync(),
            expectedArguments: ["workers.counts", 0],
            returnValue: countsJson
        );
        Assert.Single(allWorkerCounts);
        Assert.Equivalent(expectedCounts, allWorkerCounts[0]);
    }
}
