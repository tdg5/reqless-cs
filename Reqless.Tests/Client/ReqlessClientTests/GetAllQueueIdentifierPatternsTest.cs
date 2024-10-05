using Reqless.Client;
using Reqless.Tests.Common.TestHelpers;
using System.Text.Json;

namespace Reqless.Tests.Client.ReqlessClientTests;

/// <summary>
/// Unit tests for <see cref="ReqlessClient.GetAllQueueIdentifierPatternsAsync"/>.
/// </summary>
public class GetAllQueueIdentifierPatternsTest : BaseReqlessClientTest
{
    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueIdentifierPatternsAsync"/> should
    /// throw if server returns null.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsNull()
    {
        await Scenario.ThrowsWhenServerRespondsWithNullAsync(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueueIdentifierPatternsAsync(),
                expectedArguments: ["queueIdentifierPatterns.getAll", 0],
                returnValue: null
            )
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueIdentifierPatternsAsync"/> should
    /// throw if server returns JSON that can't be deserialized.
    /// </summary>
    [Fact]
    public async Task ThrowsIfServerReturnsInvalidJson()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueueIdentifierPatternsAsync(),
                expectedArguments: ["queueIdentifierPatterns.getAll", 0],
                returnValue: "null"
            )
        );
        Assert.Equal(
            "Failed to deserialize all queue identifiers JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueIdentifierPatternsAsync"/> should
    /// throw if any identifier list can't be deserialized from JSON.
    /// </summary>
    [Fact]
    public async Task ThrowsIfAnyIdentifiersListIsNotValidJson()
    {
        var exception = await Assert.ThrowsAsync<JsonException>(
            () => WithClientWithExecutorMockForExpectedArguments(
                subject => subject.GetAllQueueIdentifierPatternsAsync(),
                expectedArguments: ["queueIdentifierPatterns.getAll", 0],
                returnValue: """{"default": "null"}"""
            )
        );
        Assert.Equal(
            "Failed to deserialize queue identifier patterns JSON: null",
            exception.Message
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueIdentifierPatternsAsync"/> should
    /// throw if any identifier list contains null, empty, or only whitespace.
    /// </summary>
    [Fact]
    public async Task ThrowsIfAnyIdentifiersListContainsValueThatIsNullEmptyOrOnlyWhitespace()
    {
        await Scenario.ThrowsWhenOperationEncountersElementThatIsNullOrEmptyOrWhitespaceAsync(
            async (invalidIdentifier) =>
            {
                string?[] invalidIdentifiers = ["fine", invalidIdentifier];
                string invalidIdentifiersJson = JsonSerializer.Serialize(invalidIdentifiers);
                var returnValue = $$"""
                    {
                        "default": "[\"*\"]",
                        "other":{{JsonSerializer.Serialize(invalidIdentifiersJson)}}
                    }
                    """;
                await WithClientWithExecutorMockForExpectedArguments(
                    subject => subject.GetAllQueueIdentifierPatternsAsync(),
                    expectedArguments: ["queueIdentifierPatterns.getAll", 0],
                    returnValue: returnValue
                );
            },
            "identifierValues"
        );
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueIdentifierPatternsAsync"/> should
    /// call the executor with the expected arguments.
    /// </summary>
    [Fact]
    public async Task CallsExecutorWithExpectedArguments()
    {
        Dictionary<string, List<string>> expectedIdentifierPatterns = new()
        {
            ["default"] = ["*", "other"],
            ["something"] = ["*"],
        };
        Dictionary<string, string> returnValue = [];
        foreach (var (key, value) in expectedIdentifierPatterns)
        {
            returnValue[key] = JsonSerializer.Serialize(value);
        }
        var result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetAllQueueIdentifierPatternsAsync(),
            expectedArguments: ["queueIdentifierPatterns.getAll", 0],
            returnValue: JsonSerializer.Serialize(returnValue)
        );
        Assert.Equal(expectedIdentifierPatterns, result);
    }

    /// <summary>
    /// <see cref="ReqlessClient.GetAllQueueIdentifierPatternsAsync"/> should
    /// ignore identifiers that have no values.
    /// </summary>
    [Fact]
    public async Task IgnoresIdentifiersThatHaveNoValues()
    {
        Dictionary<string, List<string>> expectedIdentifierPatterns = new()
        {
            ["default"] = ["*", "other"],
        };
        Dictionary<string, string> returnValue = [];
        foreach (var (key, value) in expectedIdentifierPatterns)
        {
            returnValue[key] = JsonSerializer.Serialize(value);
        }
        returnValue["something"] = "[]";
        var result = await WithClientWithExecutorMockForExpectedArguments(
            subject => subject.GetAllQueueIdentifierPatternsAsync(),
            expectedArguments: ["queueIdentifierPatterns.getAll", 0],
            returnValue: JsonSerializer.Serialize(returnValue)
        );
        Assert.Equal(expectedIdentifierPatterns, result);
    }
}
