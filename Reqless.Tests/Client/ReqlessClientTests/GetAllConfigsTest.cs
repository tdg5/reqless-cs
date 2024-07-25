using System.Text.Json;
using Reqless.Client;

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
    [Fact]
    public async void CallsExecutorWithTheExpectedArguments()
    {
        Dictionary<string, JsonElement> configs = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetAllConfigsAsync(),
            expectedArguments: ["config.getAll", 0],
            returnValue: "{}"
        );
        Assert.Empty(configs);
        Assert.IsType<Dictionary<string, JsonElement>>(configs);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllConfigsAsync"/> should throw if the
    /// server returns null.
    /// </summary>
    [Fact]
    public async void ThrowsIfServerReturnsNull()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllConfigsAsync(),
                expectedArguments: ["config.getAll", 0],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllConfigsAsync"/> throws if the JSON can't
    /// be deserialized.
    /// </summary>
    [Fact]
    public async void ThrowsIfJsonCannotBeDeserialized()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllConfigsAsync(),
                expectedArguments: ["config.getAll", 0],
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize config JSON: null",
            exception.Message
        );
    }
}