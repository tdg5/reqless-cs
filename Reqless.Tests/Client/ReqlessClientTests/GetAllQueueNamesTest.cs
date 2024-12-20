using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetAllQueueNamesAsync"/>.
/// </summary>
public class GetAllQueueNamesTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueNamesAsync"/> should throw if server
    /// returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueueNamesAsync(),
                expectedArguments: ["queues.names", 0],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueNamesAsync"/> should throw if server
    /// retruns JSON that can't be deserialized.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueueNamesAsync(),
                expectedArguments: ["queues.names", 0],
                returnValue: "null"));
        Assert.Equal("Failed to deserialize all queue names JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueNamesAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ReturnsValidResultFromTheServer()
    {
        string[] expectedNames = [ExampleQueueName, "other-queue", "another-queue"];
        var namesJson = JsonSerializer.Serialize(expectedNames);
        var allQueueNames = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetAllQueueNamesAsync(),
            expectedArguments: ["queues.names", 0],
            returnValue: namesJson);
        Assert.Equivalent(expectedNames, allQueueNames);
    }
}
