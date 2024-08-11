using System.Text.Json;
using Reqless.Client;
using Reqless.Tests.TestHelpers;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetTopTagsAsync"/>.
/// </summary>
public class GetTopTagsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetTopTagsAsync"/> should throw if the given
    /// limit is not positive.
    /// </summary>
    [Fact]
    public async Task ThrowsIfLimitIsNotPositive()
    {
        await Scenario.ThrowsWhenArgumentIsNotPositiveAsync(
            (invalidLimit) => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetTopTagsAsync(limit: invalidLimit)
            ),
            "limit"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTopTagsAsync"/> should throw if server
    /// returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        var limit = 100;
        var offset = 25;
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetTopTagsAsync(limit: limit, offset: offset),
                expectedArguments: ["tags.top", 0, offset, limit],
                returnValue: null
            )
        );
        Assert.Equal("Server returned unexpected null result.", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTopTagsAsync"/> should throw if server
    /// returns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsJsonThatCannotBeDeserialized()
    {
        var limit = 10;
        var offset = 20;
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetTopTagsAsync(limit: limit, offset: offset),
                expectedArguments: ["tags.top", 0, offset, limit],
                returnValue: "null"
            )
        );
        Assert.Equal("Failed to deserialize JSON: null", exception.Message);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTopTagsAsync"/> should return a valid
    /// empty list if the server returns an empty object.
    /// </summary>
    [Fact]
    public async Task ReturnsEmptyListWhenServerReturnsEmptyObject()
    {
        var limit = 20;
        var offset = 15;
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetTopTagsAsync(limit: limit, offset: offset),
            expectedArguments: ["tags.top", 0, offset, limit],
            returnValue: "{}"
        );
        Assert.Empty(tags);
        Assert.IsType<List<string>>(tags);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetTopTagsAsync"/> should return a valid
    /// result returned by the server.
    /// </summary>
    [Fact]
    public async Task ReturnsValidResultFromTheServer()
    {
        var limit = 20;
        var offset = 15;
        string[] expectedTags = [ExampleTag, "other-tag"];
        string tagsJson = JsonSerializer.Serialize(expectedTags);
        List<string> tags = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetTopTagsAsync(limit: limit, offset: offset),
            expectedArguments: ["tags.top", 0, offset, limit],
            returnValue: tagsJson
        );
        Assert.Equivalent(expectedTags, tags);
    }
}