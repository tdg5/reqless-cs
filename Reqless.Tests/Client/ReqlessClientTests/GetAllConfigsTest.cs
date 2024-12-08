using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetAllConfigsAsync"/>.
/// </summary>
public class GetAllConfigsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetAllConfigsAsync"/> should call Executor
    /// with the expected arguments.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task CallsExecutorWithTheExpectedArguments()
    {
        Dictionary<string, string> configs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetAllConfigsAsync(),
            expectedArguments: ["config.getAll", 0],
            returnValue: "{}");
        Assert.Empty(configs);
        Assert.IsType<Dictionary<string, string>>(configs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllConfigsAsync"/> should throw if the
    /// server returns null.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllConfigsAsync(),
                expectedArguments: ["config.getAll", 0],
                returnValue: null));
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllConfigsAsync"/> throws if the JSON can't
    /// be deserialized.
    /// </summary>
    /// <returns>A task denoting the completion of the test.</returns>
    [Fact]
    public async Task ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllConfigsAsync(),
                expectedArguments: ["config.getAll", 0],
                returnValue: "null"));
        Assert.Equal(
            "Failed to deserialize config JSON: null", exception.Message);
    }
}
